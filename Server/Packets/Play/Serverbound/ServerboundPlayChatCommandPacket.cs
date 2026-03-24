using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayChatCommandPacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return Task.CompletedTask;

        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        string input = packet.ReadString();

        List<string> arguments = new List<string>();
        arguments.AddRange(input.Split(" ", StringSplitOptions.RemoveEmptyEntries));
        
        string cmd = arguments[0];
        arguments.RemoveAt(0);
        
        LogTool.Info($"{player.GetPlayerIdentifier()} used command \"{cmd}\"");
        
        // TODO: Command handler
        
        return Task.CompletedTask;
    }
}