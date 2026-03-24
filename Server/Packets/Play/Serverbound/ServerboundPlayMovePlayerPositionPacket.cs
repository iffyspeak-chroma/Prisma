using API.DataTypes;
using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayMovePlayerPositionPacket : ICallable
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