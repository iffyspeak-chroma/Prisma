using System.Net;
using API.Core.Managers;
using API.Game.Events;
using API.Logging;
using API.Protocol.Packets;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Server.Networking;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        // Start the server
        new ServerInitializer();
        
        // Load the game's registries and tags
        LogTool.Info("Loading registries... (this might take a bit)");
        new RegistryLoader();
        
        LogTool.Info("Resolving tags.");
        new TagResolver();
        
        // Next up, the packet report
        LogTool.Info("Checking for packet report.");
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