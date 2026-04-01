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
using API.Protocol.NBT;
using API.Protocol.Packets;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using fNbt;
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
        LogTool.Info("Loading registries... (this might take a bit)");
        LoadRegistryIds();
        LoadRegistryData();
        ResolveAllTags();
        
        // Next up, the packet report
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
                if (File.Exists(Constants.MojangServerFile) && details?.Downloads?.Server != null)
                {
                    var storedShaTask = HashUtil.ComputeSha1Async(Constants.MojangServerFile);
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

                if (!File.Exists(Constants.MojangServerFile))
                {
                    LogTool.Error("Failed to find server file.");
                    fileCheckCode |= (byte)FileCheckFlags.DownloadFailure;
                    return;
                }

                using var archive = ZipFile.OpenRead(Constants.MojangServerFile);
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

            if (!Directory.Exists(Constants.VanillaDirectory))
            {
                Directory.CreateDirectory(Constants.VanillaDirectory);
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
        Downloader.DownloadFileAsync(client, details.Downloads.Server.Url, Constants.MojangServerFile).Wait();
    }

    static Task DoDataGeneration(bool didUpdate = false)
    {
        if (!didUpdate)
            return Task.CompletedTask;

        LogTool.Info($"Performing data generation for version {API.Core.Server.Instance.VersionName}...");
        
        var psi = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $"-DbundlerMainClass=net.minecraft.data.Main -jar server.jar --output \"{Constants.VanillaDirectory}\" --all",
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

    static void LoadRegistryIds()
    {
        var json = File.ReadAllText(Constants.RegistriesReportFile);

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var root = JsonSerializer.Deserialize<RootDto>(json, options);

        foreach (var (registryKey, registryDto) in root!)
        {
            var registry = new Registry
            {
                RegistryId = Identifier.Parse(registryKey)
            };

            foreach (var (entryKey, entryJson) in registryDto.Entries)
            {
                var tag = NbtToolkit.ParseElement(entryJson);

                var compound = tag as NbtCompound ?? new NbtCompound("");
                
                registry.Entries.Add(new RegistryEntry
                {
                    EntryId = Identifier.Parse(entryKey),
                    Data = compound
                });
            }

            RegistryManager.Instance.Registries.Add(registry);
        }
    }

    static void LoadRegistryData()
    {
        var registries = new Dictionary<string, Registry>();

        foreach (var namespaceDir in Directory.GetDirectories(Path.Combine(Constants.VanillaDirectory, "data")))
        {
            var ns = Path.GetFileName(namespaceDir);

            foreach (var file in Directory.GetFiles(namespaceDir, "*.json", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(namespaceDir, file).Replace('\\', '/');

                if (relativePath.StartsWith("tags/"))
                    continue;

                var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? "";

                var registryId = string.IsNullOrEmpty(directory) ? $"{ns}" : $"{ns}:{directory}";
                var entryName = Path.GetFileNameWithoutExtension(file);
                var entryId = RegistryManager.FlatRegistries.Contains(directory)
                    ? $"{ns}:{entryName}"
                    : string.IsNullOrEmpty(directory)
                        ? $"{ns}:{entryName}"
                        : $"{ns}:{directory}/{entryName}";

                var json = File.ReadAllText(file);
                using var doc = JsonDocument.Parse(json);
                
                var tag = NbtToolkit.ParseElement(doc.RootElement);
                
                NbtCompound compound;
                if (tag is NbtCompound c)
                {
                    compound = c;
                }
                else
                {
                    LogTool.Warn($"Skipping file '{file}' because it is not a JSON object.");
                    continue;
                }
                
                if (!registries.TryGetValue(registryId, out var registry))
                {
                    registry = new Registry
                    {
                        RegistryId = Identifier.Parse(registryId)
                    };
                    registries.Add(registryId, registry);
                }
                
                registry.Entries.Add(new RegistryEntry
                {
                    EntryId = Identifier.Parse(entryId),
                    Data = compound
                });
            }
        }

        RegistryManager.Instance.Registries.AddRange(registries.Values);
    }

    static Dictionary<string, List<RawTagEntry>> LoadRawTags()
    {
        var result = new Dictionary<string, List<RawTagEntry>>();

        var ns = "minecraft";

        var tagsDir = Path.Combine(Directory.GetDirectories(Path.Combine(Constants.VanillaDirectory, "data", "minecraft", "tags")));

        if (!Directory.Exists(tagsDir))
            return result;

        foreach (var file in Directory.GetFiles(tagsDir, "*.json", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(tagsDir, file).Replace('\\', '/');
            var directory = Path.GetDirectoryName(relativePath)!;

            var registryId = $"{ns}:{directory}";

            var tagName = Path.GetFileNameWithoutExtension(file);
            var tagId = $"{ns}:{tagName}";

            using var doc = JsonDocument.Parse(File.ReadAllText(file));

            var rawTag = new RawTagEntry
            {
                Id = Identifier.Parse(tagId)
            };

            foreach (var val in doc.RootElement.GetProperty("values").EnumerateArray())
            {
                rawTag.Values.Add(val.GetString()!);
            }

            if (!result.TryGetValue(registryId, out var list))
            {
                list = new List<RawTagEntry>();
                result[registryId] = list;
            }
            
            list.Add(rawTag);
        }

        return result;
    }

    static List<int> ResolveTag(RawTagEntry tag, string registryId,
        Dictionary<string, int> entryLookup, Dictionary<string, RawTagEntry> tagLookup,
        HashSet<string> visited)
    {
        var result = new List<int>();

        if (!visited.Add(tag.Id.ToString()))
            return result;

        foreach (var value in tag.Values)
        {
            if (value.StartsWith("#"))
            {
                var tagId = value.Substring(1);

                if (tagLookup.TryGetValue(tagId, out var nested))
                {
                    result.AddRange(ResolveTag(nested, registryId, entryLookup, tagLookup, visited));
                }
                else
                {
                    LogTool.Warn($"Missing tag '{tagId}' in registry '{registryId}'");
                }

                continue;
            }

            if (entryLookup.TryGetValue(value, out var protocolId))
            {
                result.Add(protocolId);
            }
            else
            {
                LogTool.Warn($"Missing entry '{value}' in registry '{registryId}'");
            }
        }

        return result;
    }

    static void ResolveAllTags()
    {
        var rawTagsByRegistry = LoadRawTags();

        foreach (var (registryId, rawTags) in rawTagsByRegistry)
        {
            var registry =
                RegistryManager.Instance.Registries.FirstOrDefault(r => r.RegistryId.ToString() == registryId);

            if (registry == null)
            {
                LogTool.Warn($"Registry '{registryId}' not found for tags, skipping.");
                continue;
            }

            var entryLookup = registry.Entries.ToDictionary(
                e => e.EntryId.ToString(), e => ((NbtInt)e.Data["protocol_id"]).Value);

            var tagLookup = rawTags.ToDictionary(t => t.Id.ToString());

            foreach (var rawTag in rawTags)
            {
                var resolved = ResolveTag(rawTag, registryId, entryLookup, tagLookup, new HashSet<string>());
                
                RegistryManager.Instance.Tags.Add(new TagEntry
                {
                    TagName = rawTag.Id,
                    Entries = resolved.Distinct().ToList()
                });
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