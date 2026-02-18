using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Login.Serverbound;

public class ServerboundLoginAcknowledgePacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        client.Gamestate = PlayerGamestate.Configuration;
        
        LogTool.Info($"Configuring data for {NetworkedClient.GetPlayerIdentifier(player)}...");
    }
}