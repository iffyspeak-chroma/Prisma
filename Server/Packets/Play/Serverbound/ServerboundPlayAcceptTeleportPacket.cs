using API.Player;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

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