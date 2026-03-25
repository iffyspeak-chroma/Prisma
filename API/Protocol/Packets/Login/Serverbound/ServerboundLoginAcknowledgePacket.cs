using API.Core.Managers;
using API.Logging;
using API.Player;
using API.Player.State;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Login.Serverbound;

public class ServerboundLoginAcknowledgePacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        client.Gamestate = PlayerGamestate.Configuration;

        client.TransmitHeartbeat += DoHeartbeat;
        client.StartHeartbeat();
        
        LogTool.Info($"Configuring data for {player.GetPlayerIdentifier()}...");

        return Task.CompletedTask;
    }

    private void DoHeartbeat(NetworkedClient client)
    {
        _ = PlayerManager.SendHeartbeatAsync(client);
    }
}