using API.Core;
using API.Core.Managers;
using API.Core.Transform;
using API.Game;
using API.Player;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlaySynchronizePlayerPositionPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        if (packet == null)
        {
            // A teleport was not already specified so just create one
            packet = new Packet();
            
            packet.Write(EntityManager.Instance.GlobalTeleportId);
            player.RecentTeleportId = EntityManager.Instance.GlobalTeleportId;
            EntityManager.Instance.GlobalTeleportId++;

            // TODO: Proper implementation
            // Send them to wherever their spawn is or world spawn if they don't have one
            Location spawn = new Location(0d, 64d, 0d);
            spawn.Serialize(packet);
            
            // Set their entity location too :)
            player.AssociatedEntity?.Location = spawn;
            
            // TODO: Proper implementation part 2: electric boogaloo
            // TODO: Don't use Location for this kinda thing, even if convinient for now.
            // Set their velocity (because it has no parameters, it'll be zero'd out)
            Location velocity = new Location();
            velocity.Serialize(packet);

            player.AssociatedEntity?.Velocity = velocity;

            // TODO: I mean again, do this properly. Read it from the world file or something.
            Heading facing = new Heading();
            facing.Serialize(packet);

            player.AssociatedEntity?.LookDirection = facing;

            TeleportFlags flags = TeleportFlags.None;
            packet.Write((int) flags, asVarInt: false);
        }
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_position"].Id);
        packet.WriteLength();

        await client.SendPacket(packet);

        player.AcceptMovementPackets = false;
    }
}