using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationPluginMessagePacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
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
            LogTool.Warn($"{NetworkedClient.GetPlayerIdentifier(player)} sent {identifier} but it could not be found.");
        }
    }
}