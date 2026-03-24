using API.DataTypes.Entities;
using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayMovePlayerRotationPacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // https://minecraft.wiki/w/Java_Edition_protocol/Packets#Set_Player_Rotation
        if (packet == null)
            return Task.CompletedTask;

        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        float yaw = packet.ReadFloat();
        float pitch = packet.ReadFloat();
        
        // TODO: Probably find something to do with these flags.
        RotationFlags flags = (RotationFlags) packet.ReadByte();

        if (player.AssociatedEntity != null)
        {
            player.AssociatedEntity.LookDirection = new Heading(yaw, pitch);
        }
        
        return Task.CompletedTask;
    }
}