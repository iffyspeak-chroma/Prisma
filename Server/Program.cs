using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using API.Core;
using API.Core.Managers;
using API.DataPacks;
using API.Game.Events;
using API.Logging;
using API.Protocol.Mojang;
using API.Protocol.Packets;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Server.Networking;
using Server.Updater;

namespace Server;

class Program
{
    static byte fileCheckCode = (byte) FileCheckFlags.Nothing;
    
    static void Main(string[] args)
    {
        // Start by checking if the server has run before
        if (FirstLaunch())
        {
            // So the server hasn't been run before. We should do the first setups before continuing.
            Setup();
        }
        
        // Give the server a new instance.
        API.Core.Server.Instance = new API.Core.Server();
        
        // NOW, we have the fun task of loading the configuration.
        try
        {
            if (!File.Exists(Constants.ConfigurationFile))
            {
                UseDefaultConfiguration();
            }

            string json = File.ReadAllText(Constants.ConfigurationFile);

            ServerConfiguration? config = JsonSerializer.Deserialize<ServerConfiguration>(json);

            if (config == null)
            {
                UseDefaultConfiguration();
                return;
            }

            API.Core.Server.Instance.Configuration = config;
        }
        finally
        {
            Debug.Assert(API.Core.Server.Instance.Configuration != null, "Server.Instance.Configuration != null");
            LogTool.Info("Configuration loaded!");
            //LogTool.Debug("Debug messages are enabled!");
        }
        
        // The config and packet report check.
        RequiredFileCheck();
        if (fileCheckCode != 0)
        {
            LogTool.Error("Missing required files in configuration folder!");
            Environment.Exit(fileCheckCode);
        }
        
        // Load the game's registries and tags
        API.Core.Server.Instance.GameData = new GameData();
        LoadRegistries(API.Core.Server.Instance.GameData);
        LoadTags(API.Core.Server.Instance.GameData);
        ResolveAllTags(API.Core.Server.Instance.GameData);
        
        // Next up, the packet report. It's pretty much the same thing but now for the packet report.
        try
        {
            if (!File.Exists(Constants.PacketReportFile))
            {
                LogTool.Error("Missing packet report! (This isn't possible in theory but has occurred anyways.) Exiting!");
                Environment.Exit(1);
            }

            PacketReport report = new PacketReport();
            report.Load(Constants.PacketReportFile);

            API.Core.Server.Instance.PacketReport = report;
        }
        finally
        {
            LogTool.Info("Packet report loaded!");
        }
        
        // Initialize the PacketManager's PacketList
        PacketManager.Instance.InitializePacketList();
        
        // Initialize the EventDispatcher
        API.Core.Server.Instance.EventDispatcher = new EventDispatcher();
        
        // It's officially time to try and start the server. For realsies now.
        StartServerAsync().Wait();
    }
    
    static bool FirstLaunch()
    {
        if (Directory.Exists(Constants.ConfigurationDirectory))
        {
            // It likely has been launched before and should be okay to continue. Without any further setup
            return false;
        }

        return true;
    }

    static void Setup()
    {
        try
        {
            // Create all necessary directories
            // For now, this is technically the only one we'll need
            Directory.CreateDirectory(Constants.ConfigurationDirectory);
            
            // Create them a default configuration file!
            var json = JsonSerializer.Serialize(new ServerConfiguration(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.ConfigurationFile, json);
        }
        finally
        {
            LogTool.Info("Because this is your first time running Prisma: you should configure your server then launch Prisma again.");
            LogTool.Info("Exiting!");
            Environment.Exit(0);
        }
    }

    static void RequiredFileCheck()
    {
        bool downloadedUpdate = false;

        try
        {
            Debug.Assert(API.Core.Server.Instance != null, "API.Core.Server.Instance != null");
            Debug.Assert(API.Core.Server.Instance.Configuration != null, "API.Core.Server.Instance.Configuration != null");
            if (!API.Core.Server.Instance.Configuration.SkipUpdaterStep)
            {
                LogTool.Info("Retrieving version manifest...");
                var http = new HttpClient();
                var manifestTask = http.GetFromJsonAsync<VersionManifest>(
                "https://launchermeta.mojang.com/mc/game/version_manifest.json"
                );

                manifestTask.Wait();

                var manifest = manifestTask.Result;

                if (manifest == null)
                {
                    LogTool.Error("Failed to get the version manifest from online.");
                    fileCheckCode |= (byte)FileCheckFlags.VersionManifest;
                    return;
                }
                
                var latestR = manifest.Versions.FirstOrDefault(v => v.Id == manifest.Latest.Release);
                var latestS = manifest.Versions.FirstOrDefault(v => v.Id == manifest.Latest.Snapshot);

                if (latestR == null || latestS == null)
                {
                    LogTool.Error("Release and snapshot data are null.");
                    fileCheckCode |= (byte)FileCheckFlags.VersionData;
                    return;
                }
                
                LogTool.Info($"Detected latest release as {latestR.Id} and latest snapshot as {latestS.Id}");

                Debug.Assert(API.Core.Server.Instance != null, "API.Core.Server.Instance != null");
                Debug.Assert(API.Core.Server.Instance.Configuration != null, "API.Core.Server.Instance.Configuration != null");
                var versionTask =
                    http.GetFromJsonAsync<VersionDetails>(API.Core.Server.Instance.Configuration.PreferSnapshot
                        ? latestS.Url
                        : latestR.Url);

                versionTask.Wait();

                var details = versionTask.Result;

                if (details == null)
                {
                    LogTool.Error("Failed to get the version details from online.");
                    fileCheckCode |= (byte)FileCheckFlags.VersionData;
                    return;
                }

                if (!Directory.Exists(Constants.VersionDirectory))
                    Directory.CreateDirectory(Constants.VersionDirectory);
                
                LogTool.Info("Checking version...");
                if (File.Exists(Constants.PackedServerFile) && details?.Downloads?.Server != null)
                {
                    var storedShaTask = HashUtil.ComputeSha1Async(Constants.PackedServerFile);
                    storedShaTask.Wait();
                    var storedSha = storedShaTask.Result;
                    var remoteSha = details.Downloads.Server.Sha1;

                    if (storedSha == remoteSha)
                    {
                        LogTool.Info("Version is up to date!");
                    }
                    else
                    {
                        LogTool.Info("Version is outdated. Updating!");
                        DownloadServerFile(http, details);
                        downloadedUpdate = true;
                    }
                }
                else
                {
                    if (details != null)
                    {
                        LogTool.Info("No version detected. Downloading!");
                        DownloadServerFile(http, details);
                        downloadedUpdate = true;
                    }
                }

                if (!File.Exists(Constants.PackedServerFile))
                {
                    LogTool.Error("Failed to find server file.");
                    fileCheckCode |= (byte)FileCheckFlags.DownloadFailure;
                    return;
                }

                using var archive = ZipFile.OpenRead(Constants.PackedServerFile);
                var entry = archive.GetEntry("version.json");

                if (entry == null)
                {
                    LogTool.Error("Failed to get version information.");
                    fileCheckCode |= (byte)FileCheckFlags.InformationExtractionFailure;
                    return;
                }
                
                using var stream = entry.Open();
                var versionInfo = JsonSerializer.Deserialize<ServerVersionInfo>(stream);

                if (versionInfo == null)
                {
                    LogTool.Error("Version information is null.");
                    fileCheckCode |= (byte)FileCheckFlags.InformationExtractionFailure;
                    return;
                }

                API.Core.Server.Instance.VersionName = versionInfo.Name;
                API.Core.Server.Instance.ProtocolId = versionInfo.ProtocolVersion;
            }

            if (!Directory.Exists(Constants.MinecraftDataDirectory))
            {
                Directory.CreateDirectory(Constants.MinecraftDataDirectory);
                // Even if we didn't have to update the file, the directory could've been deleted.
                downloadedUpdate = true;
            }
            
            DoDataGeneration(downloadedUpdate).Wait();

            if (!File.Exists(Constants.PacketReportFile))
            {
                fileCheckCode |= (byte)FileCheckFlags.PacketReport;
            }
        }
        catch (Exception e)
        {
            LogTool.Exception(e);
        }
    }

    static void UseDefaultConfiguration()
    {
        if (API.Core.Server.Instance != null)
        {
            API.Core.Server.Instance.Configuration = new ServerConfiguration();
            LogTool.Warn("Missing configuration file! Using default values.");
            
            // Quickly, save a default copy.
            var json = JsonSerializer.Serialize(new ServerConfiguration(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.ConfigurationFile, json);
            
            return;
        }
        
        LogTool.Error("Server instance is not initialized yet! (In theory, this shouldn't be possible. You messed up.)");
    }

    static void DownloadServerFile(HttpClient client, VersionDetails details)
    {
        Downloader.DownloadFileAsync(client, details.Downloads.Server.Url, Constants.PackedServerFile).Wait();
    }

    static Task DoDataGeneration(bool didUpdate = false)
    {
        if (!didUpdate)
            return Task.CompletedTask;

        LogTool.Info($"Performing data generation for version {API.Core.Server.Instance.VersionName}...");
        
        var psi = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-DbundlerMainClass=net.minecraft.data.Main -jar server.jar --output \"{Constants.MinecraftDataDirectory}\" --all",
            WorkingDirectory = Constants.VersionDirectory,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();

        if (process?.ExitCode != 0)
        {
            LogTool.Error("Data generation failure.");
            fileCheckCode |= (byte)FileCheckFlags.DataGenerationFailure;
        }

        return Task.CompletedTask;
    }

    static void LoadRegistries(GameData gameData)
    {
        var dataRoot = Path.Combine(Constants.MinecraftDataDirectory, "data", "minecraft");

        foreach (var dir in Directory.EnumerateDirectories(dataRoot))
        {
            var folderName = Path.GetFileName(dir);

            if (folderName == "tags")
                continue;

            var identifier = new Identifier("minecraft", folderName);

            var registry = new Registry
            {
                Identifier = identifier
            };

            foreach (var file in Directory.EnumerateFiles(dir, "*.json", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(dir, file)
                    .Replace('\\', '/')
                    .Replace(".json", "");

                var entryId = new Identifier("minecraft", relative);

                registry.Entries[entryId] = new RegistryEntry
                {
                    Id = entryId,
                    RawJson = File.ReadAllText(file)
                };
            }

            gameData.Registries[identifier] = registry;
        }
    }
    
    static void LoadTags(GameData gameData)
    {
        var tagsRoot = Path.Combine(Constants.MinecraftDataDirectory, "data", "minecraft", "tags");

        foreach (var registryDir in Directory.EnumerateDirectories(tagsRoot))
        {
            var folderName = Path.GetFileName(registryDir);

            var registryName = folderName switch
            {
                "blocks" => new Identifier("minecraft", "block"),
                "items" => new Identifier("minecraft", "item"),
                "fluids" => new Identifier("minecraft", "fluid"),
                "entity_types" => new Identifier("minecraft", "entity_type"),
                _ => new Identifier("minecraft", folderName)
            };

            var group = new TagGroup
            {
                RegistryId = registryName
            };

            foreach (var file in Directory.EnumerateFiles(registryDir, "*.json", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(registryDir, file)
                    .Replace('\\', '/')
                    .Replace(".json", "");

                var tagId = new Identifier("minecraft", relative);

                using var doc = JsonDocument.Parse(File.ReadAllText(file));

                var values = new List<TagValue>();

                if (doc.RootElement.TryGetProperty("values", out var arr))
                {
                    foreach (var v in arr.EnumerateArray())
                    {
                        var raw = v.GetString();

                        if (raw.StartsWith("#"))
                        {
                            values.Add(new TagValue
                            {
                                IsTag = true,
                                Id = Identifier.Parse(raw.Substring(1))
                            });
                        }
                        else
                        {
                            values.Add(new TagValue
                            {
                                IsTag = false,
                                Id = Identifier.Parse(raw)
                            });
                        }
                    }
                }

                group.Tags[tagId] = new Tag
                {
                    Id = tagId,
                    Values = values
                };
            }

            gameData.TagGroups[registryName] = group;
        }
    }

    static HashSet<Identifier> ResolveTag(TagGroup group, Identifier tagId, HashSet<Identifier> visiting)
    {
        if (!group.Tags.TryGetValue(tagId, out var tag))
            throw new Exception($"Unknown tag: {tagId}");
        
        if (tag.Resolved.Count > 0)
            return tag.Resolved;
        
        if (visiting.Contains(tagId))
            throw new Exception($"Tag cycle detected: {tagId}");

        visiting.Add(tagId);

        var result = new HashSet<Identifier>();

        foreach (var value in tag.Values)
        {
            if (value.IsTag)
            {
                var nested = ResolveTag(group, value.Id, visiting);

                foreach (var id in nested)
                    result.Add(id);
            }
            else
            {
                result.Add(value.Id);
            }
        }

        visiting.Remove(tagId);

        tag.Resolved = result;
        return result;
    }

    static void ResolveAllTags(GameData gameData)
    {
        foreach (var group in gameData.TagGroups.Values)
        {
            foreach (var tagId in group.Tags.Keys)
            {
                ResolveTag(group, tagId, new HashSet<Identifier>());
            }
        }
    }

    static async Task StartServerAsync()
    {
        IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
        IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();

        try
        {
            var bootstrap = new ServerBootstrap();
            bootstrap.Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 128)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("handler", new AsyncReceivedMessageHandler(API.Core.Server.Instance!.EventDispatcher));
                }));

            if (API.Core.Server.Instance == null)
            {
                LogTool.Error("Server instance is not initialized yet! (How did you manage to get here?)");
                return;
            }

            if (API.Core.Server.Instance.Configuration == null)
            {
                LogTool.Error("Server configuration is not initialized yet! (How did you manage to get here?)");
                return;
            }

            IPAddress bindAddress = API.Core.Server.Instance.Configuration.BindAddress == "0.0.0.0"
                ? IPAddress.Any
                : IPAddress.Parse(API.Core.Server.Instance.Configuration.BindAddress);

            try
            {
                var serverBind = await bootstrap.BindAsync(bindAddress, API.Core.Server.Instance.Configuration.Port);
                LogTool.Info($"Server started successfully @ {bindAddress}:{API.Core.Server.Instance.Configuration.Port}!");

                await serverBind.CloseCompletion;
            }
            catch (Exception ex)
            {
                LogTool.Exception(ex);
            }
        }
        finally
        {
            await Task.WhenAll(
                bossGroup.ShutdownGracefullyAsync(),
                workerGroup.ShutdownGracefullyAsync()
            );
        }
    }
}