using System.Buffers.Binary;
using API.Protocol.Packets;

namespace API.Game.World.Chunk;

public class ChunkSection : ISerializable
{
    public short BlockCount;
    public PalettedContainer Blocks { get; }
    public PalettedContainer Biomes { get; }

    public ChunkSection()
    {
        Blocks = new PalettedContainer(16 * 16 * 16, true);
        Biomes = new PalettedContainer(4 * 4 * 4, false);
    }

    public void Serialize(Packet packet)
    {
        packet.Write(BinaryPrimitives.ReverseEndianness(BlockCount));
        
        Blocks.Serialize(packet);
        Biomes.Serialize(packet);
    }
}