using System.Buffers.Binary;
using API.Entities;
using API.Protocol.NBT;
using API.Protocol.Packets;
using fNbt;

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
            
            foreach (ChunkSection section in chunk.Sections)
            {
                section.Serialize(p);
            }

            p.Write(chunk.BlockEntities.Count);
            foreach (BlockEntity be in chunk.BlockEntities)
            {
                byte xzPacked = (byte) (((be.X & 15) << 4) | (be.Z & 15));
                p.Write(xzPacked);
                p.Write(BinaryPrimitives.ReverseEndianness(be.Y));
                p.Write(be.Id);

                NbtFile blockData = new NbtFile();
                if (be.Data == null)
                {
                    be.Data = new NbtCompound("");
                }

                blockData.RootTag = be.Data;

                byte[] rawData = NbtToolkit.StripUnnecessary(blockData.SaveToBuffer(NbtCompression.None));
                
                p.Write(rawData);
            }
            
            // Lightmap Yagami vs (L)ightmap Lawliet isn't even close.
            chunk.Lightmap.Serialize(p);
            
            return p.ToArray() ?? throw new InvalidOperationException("Serialization array is null!");
        }
    }
}