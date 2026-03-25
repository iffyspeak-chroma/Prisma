using API.Core.Managers;
using API.Player.Abilities;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayPlayerAbilitiesPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_abilities"].Id);
            packet.WriteLength();

            await client.SendPacket(packet);

            return;
        }
        
        CheatAbilities abilities = CheatAbilities.Invulnerable | CheatAbilities.PermitFlying;

        
        packet.Write((byte) abilities);

        // Flight speed
        packet.Write(0.05f);
            
        // FOV Mod
        packet.Write(0.1f);
            
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_abilities"].Id);
        packet.WriteLength();

        await client.SendPacket(packet);
            
        await new ClientboundPlaySetHeldSlotPacket().Call(context, null);
    }
}