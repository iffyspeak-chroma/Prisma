using API.Entities;

namespace API.Game.World.Chunk;

public class Chunk
{
    public int X { get; }
    public int Z { get; }
    public Heightmap[] Heightmaps { get; }
    public ChunkSection[] Sections { get; }
    public List<BlockEntity> BlockEntities { get; } = new();

    public Chunk(int x, int z, int worldHeight)
    {
        X = x;
        Z = z;
        
        Heightmaps = new Heightmap[3];
        Heightmaps[0] = new Heightmap(worldHeight, HeightmapType.WORLD_SURFACE);
        Heightmaps[1] = new Heightmap(worldHeight, HeightmapType.MOTION_BLOCKING);
        Heightmaps[2] = new Heightmap(worldHeight, HeightmapType.MOTION_BLOCKING_NO_LEAVES);

        int sectionCount = worldHeight / 16;
        Sections = new ChunkSection[sectionCount];

        for (int i = 0; i < sectionCount; i++)
        {
            Sections[i] = new ChunkSection();
        }
    }
}