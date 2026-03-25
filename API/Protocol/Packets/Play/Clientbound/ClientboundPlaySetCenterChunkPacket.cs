using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlaySetCenterChunkPacket : ICallablePacket
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_chunk_cache_center"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        // Center Chunk X/Z
        packet.Write(0);
        packet.Write(0);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_chunk_cache_center"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
    }
}