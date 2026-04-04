namespace API.Game.World.Chunk;

public class ChunkSection
{
    public short BlockCount;
    public PalettedContainer Blocks { get; }
    public PalettedContainer Biomes { get; }

    public ChunkSection()
    {
        Blocks = new PalettedContainer(16 * 16 * 16, true);
        Biomes = new PalettedContainer(4 * 4 * 4, false);
    }
}