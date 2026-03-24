using API.Logging;
using API.Player;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationPluginMessagePacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        string identifier = packet.ReadString();

        if (player.PluginMessages.ContainsKey(identifier))
        {
            player.PluginMessages[identifier] = packet.ReadBytes(packet.UnreadLength());
        }
        else
        {
            LogTool.Warn($"{player.GetPlayerIdentifier()} sent {identifier} but it could not be found.");
        }

        return Task.CompletedTask;
    }
}