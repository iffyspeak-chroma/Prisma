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
        CheatAbilities abilities = CheatAbilities.Invulnerable | CheatAbilities.PermitFlying;

        using (Packet p = new Packet())
        {
            p.Write((byte) abilities);

            // Flight speed
            p.Write(0.05f);
            
            // FOV Mod
            p.Write(0.1f);
            
            p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_abilities"].Id);
            p.WriteLength();

            await client.SendPacket(p);
            
            await new ClientboundPlaySetHeldSlotPacket().Call(context, null);
        }
    }
}