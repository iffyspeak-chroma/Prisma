using System.Net;
using System.Text.Json;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Server.ConfigurationTools;
using Server.Networking;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        // Start by checking if the server has run before
        if (FirstLaunch())
        {
            // So the server hasn't been run before. We should do the first setups before continuing.
            Setup();
        }

        // The config and packet report check.
        if (RequiredFileCheck() != 0)
        {
            LogTool.Error("Missing required files in configuration folder!");
            Environment.Exit(RequiredFileCheck());
        }
        
        // Give the server a new instance.
        Server.Instance = new Server();
        
        // NOW, we have the fun task of loading the configuration.
        try
        {
            if (!File.Exists(Constants.ConfigurationFile))
            {
                UseDefaultConfiguration();
            }

            string json = File.ReadAllText(Constants.ConfigurationFile);

            ConfigMapping? config = JsonSerializer.Deserialize<ConfigMapping>(json);

            if (config == null)
            {
                UseDefaultConfiguration();
                return;
            }

            Server.Instance.Configuration = config;
        }
        finally
        {
            LogTool.Info("Configuration loaded!");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            LogTool.Debug("Debug messages are enabled!", Server.Instance.Configuration.DebugMode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        
        // Next up, the packet report. It's pretty much the same thing but now for the packet report.
        try
        {
            if (!File.Exists(Constants.ConfigurationFile))
            {
                LogTool.Error("Missing packet report! (This isn't possible in theory but has occurred anyways.) Exiting!");
                Environment.Exit(1);
            }

            string json = File.ReadAllText(Constants.ConfigurationFile);

            PacketReport? report = JsonSerializer.Deserialize<PacketReport>(json);

            if (report == null)
            {
                LogTool.Error("Packet report potentially isn't valid. Exiting!");
                Environment.Exit(1);
            }

            Server.Instance.PacketReport = report;
        }
        finally
        {
            LogTool.Info("Packet report loaded!");
        }
        
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
            var json = JsonSerializer.Serialize(new ConfigMapping(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.ConfigurationFile, json);
        }
        finally
        {
            LogTool.Info("Because this is your first time running Prisma: you should configure your server then launch Prisma again.");
            LogTool.Info("Exiting!");
            Environment.Exit(0);
        }
    }

    static int RequiredFileCheck()
    {
        // Even if we aren't checking for configuration here anymore, I want to keep this an int in the event we need to
        // add more files to it.
        const int packetsReport = 0b00000001;
        
        // Dependent on what's missing, it'll return that code.
        return File.Exists(Constants.PacketReportFile) ? 0 : packetsReport;
    }

    static void UseDefaultConfiguration()
    {
        if (Server.Instance != null)
        {
            Server.Instance.Configuration = new ConfigMapping();
            LogTool.Warn("Missing configuration file! Using default values.");
            
            // Quickly, save a default copy.
            var json = JsonSerializer.Serialize(new ConfigMapping(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Constants.ConfigurationFile, json);
            
            return;
        }
        
        LogTool.Error("Server instance is not initialized yet! (In theory, this shouldn't be possible. You messed up.)");
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
                    pipeline.AddLast("handler", new ReceivedMessageHandler());
                }));

            if (Server.Instance == null)
            {
                LogTool.Error("Server instance is not initialized yet! (How did you manage to get here?)");
                return;
            }

            if (Server.Instance.Configuration == null)
            {
                LogTool.Error("Server configuration is not initialized yet! (How did you manage to get here?)");
                return;
            }

            IPAddress bindAddress = Server.Instance.Configuration.BindAddress == "0.0.0.0"
                ? IPAddress.Any
                : IPAddress.Parse(Server.Instance.Configuration.BindAddress);

            var serverBind = await bootstrap.BindAsync(bindAddress, Server.Instance.Configuration.Port);
            LogTool.Info(
                $"Server started successfully @ {bindAddress.ToString()}:{Server.Instance.Configuration.Port}!");

            await serverBind.CloseCompletion;
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