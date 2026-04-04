using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;
using API.Core;
using API.Logging;
using Server.Updater;

namespace Server;

public class ServerInitializer
{
    private byte FileCheckCode;
    
    public ServerInitializer()
    {
        FileCheckCode = (byte) FileCheckFlags.Nothing;

        if (FirstLaunch())
            Setup();
        
        API.Core.Server.Instance = new API.Core.Server();
        
        LoadConfiguration();
        
        RequiredFileCheck();

        if (FileCheckCode != 0)
        {
            LogTool.Error("Missing required files in configuration folder!");
            Environment.Exit(FileCheckCode);
        }
    }
    
    bool FirstLaunch()
    {
        if (Directory.Exists(Constants.ConfigurationDirectory))
        {
            // It likely has been launched before and should be okay to continue. Without any further setup
            return false;
        }

        return true;
    }
    
    void Setup()
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
    
    void RequiredFileCheck()
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
                    FileCheckCode |= (byte)FileCheckFlags.VersionManifest;
                    return;
                }
                
                var latestR = manifest.Versions.FirstOrDefault(v => v.Id == manifest.Latest.Release);
                var latestS = manifest.Versions.FirstOrDefault(v => v.Id == manifest.Latest.Snapshot);

                if (latestR == null || latestS == null)
                {
                    LogTool.Error("Release and snapshot data are null.");
                    FileCheckCode |= (byte)FileCheckFlags.VersionData;
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
                    FileCheckCode |= (byte)FileCheckFlags.VersionData;
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
                    FileCheckCode |= (byte)FileCheckFlags.DownloadFailure;
                    return;
                }

                using var archive = ZipFile.OpenRead(Constants.MojangServerFile);
                var entry = archive.GetEntry("version.json");

                if (entry == null)
                {
                    LogTool.Error("Failed to get version information.");
                    FileCheckCode |= (byte)FileCheckFlags.InformationExtractionFailure;
                    return;
                }
                
                using var stream = entry.Open();
                var versionInfo = JsonSerializer.Deserialize<ServerVersionInfo>(stream);

                if (versionInfo == null)
                {
                    LogTool.Error("Version information is null.");
                    FileCheckCode |= (byte)FileCheckFlags.InformationExtractionFailure;
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
                FileCheckCode |= (byte)FileCheckFlags.PacketReport;
            }
        }
        catch (Exception e)
        {
            LogTool.Exception(e);
        }
    }
    
    void UseDefaultConfiguration()
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
    
    void DownloadServerFile(HttpClient client, VersionDetails details)
    {
        Downloader.DownloadFileAsync(client, details.Downloads.Server.Url, Constants.MojangServerFile).Wait();
    }

    Task DoDataGeneration(bool didUpdate = false)
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
            FileCheckCode |= (byte)FileCheckFlags.DataGenerationFailure;
        }

        return Task.CompletedTask;
    }

    void LoadConfiguration()
    {
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
        }
    }
}