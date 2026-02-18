using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationAcknowledgeFinishPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = new ServerPlayer();

        client.Gamestate = PlayerGamestate.Play;
        LogTool.Info($"{NetworkedClient.GetPlayerIdentifier(player)} finished configuration!");
    }
}