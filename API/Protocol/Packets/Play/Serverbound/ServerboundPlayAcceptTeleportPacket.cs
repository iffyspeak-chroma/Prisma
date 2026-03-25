using API.Core.Managers;
using API.Player;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Serverbound;

public class ServerboundPlayAcceptTeleportPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        int teleportId = packet.ReadVarInt();

        if (player.RecentTeleportId == teleportId)
            player.AcceptMovementPackets = true;

        return Task.CompletedTask;
    }
}