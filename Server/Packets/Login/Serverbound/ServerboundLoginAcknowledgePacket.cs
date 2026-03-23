using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Login.Serverbound;

public class ServerboundLoginAcknowledgePacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        client.Gamestate = PlayerGamestate.Configuration;
        
        LogTool.Info($"Configuring data for {NetworkedClient.GetPlayerIdentifier(player)}...");

        return Task.CompletedTask;
    }
}