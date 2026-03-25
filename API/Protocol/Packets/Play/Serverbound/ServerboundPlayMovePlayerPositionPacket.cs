using API.Core.Managers;
using API.Core.Transform;
using API.Player;
using API.Player.Abilities;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Serverbound;

public class ServerboundPlayMovePlayerPositionPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return Task.CompletedTask;

        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        if (player.AssociatedEntity == null)
            return Task.CompletedTask;
        
        player.AssociatedEntity.Location = new Location(packet.ReadDouble(), packet.ReadDouble(), packet.ReadDouble());
        MovementFlags flags = (MovementFlags) packet.ReadByte();
        
        return Task.CompletedTask;
    }
}