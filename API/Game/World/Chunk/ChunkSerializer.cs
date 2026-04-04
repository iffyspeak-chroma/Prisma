using API.Protocol.Packets;

namespace API.Game.World.Chunk;

public class ChunkSerializer
{
    public static byte[] Serialize(Chunk chunk)
    {
        using (Packet p = new Packet())
        {
            p.Write(chunk.X, asVarInt: false);
            p.Write(chunk.Z, asVarInt: false);
            
            p.Write(chunk.Heightmaps.Length);
            foreach (Heightmap heightmap in chunk.Heightmaps)
            {
                p.Write((int) heightmap.Type);
                p.Write(heightmap.GetRaw().Length);
                foreach (long l in heightmap.GetRaw())
                {
                    p.Write(l, asVarLong: false);
                }
            }
            
            // TODO: Chunk Data, Block Entities, Light
            
            return p.ToArray() ?? throw new InvalidOperationException("Serialization array is null!");
        }
    }
}