using API.Core.Managers;
using API.Game.World;
using API.Game.World.ChunkData;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayLevelChunkWithLightPacket : ICallablePacket
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:level_chunk_with_light"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }

        byte viewDistance = client.Player.ClientSettings?.ViewDistance ?? 16;
        World world = new World();

        int playerChunkX = (int) client.Player.AssociatedEntity?.Location.X! / 16;
        int playerChunkZ = (int) client.Player.AssociatedEntity?.Location.Z! / 16;
        
        for (int dx = -viewDistance; dx <= viewDistance; dx++)
        for (int dz = -viewDistance; dz <= viewDistance; dz++)
        {
            int chunkX = playerChunkX + dx;
            int chunkZ = playerChunkZ + dz;

            Chunk chunk = world.GetOrCreate(chunkX, chunkZ);
            byte[] chunkData = ChunkSerializer.Serialize(chunk);
            
            packet.Write(chunkData);
            
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:level_chunk_with_light"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);

            packet = new Packet();
        }
    }
}