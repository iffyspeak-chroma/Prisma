using API.Core.Managers;
using API.Core.Transform;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayDefaultSpawnPositionPacket : ICallablePacket
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_default_spawn_position"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        packet.Write("minecraft:overworld");
        
        packet.Write(new Position(0, 64, 0));
        
        packet.Write(0.0f);
        packet.Write(0.0f);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_default_spawn_position"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
        
        await new ClientboundPlayGameEventPacket().Call(context, null);
    }
}