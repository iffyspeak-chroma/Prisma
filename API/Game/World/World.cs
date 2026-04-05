using API.Core;
using API.Core.Managers;
using API.Game.World.ChunkData;
using API.Protocol.Mojang;
using fNbt;

namespace API.Game.World;

public class World
{
    private readonly Dictionary<(int x, int z), Chunk> chunks = new();

    public Chunk GetOrCreate(int x, int z)
    {
        if (!chunks.TryGetValue((x, z), out var chunk))
        {
            chunk = GenerateChunk(x, z);
            chunks[(x, z)] = chunk;
        }

        return chunk;
    }

    private Chunk GenerateChunk(int chunkX, int chunkZ)
    {
        // TODO: Don't assume its overworld
        int? worldHeight = GetWorldHeight(Identifier.Parse("minecraft:overworld"));
        
        if (worldHeight == null)
            worldHeight = 128;
        
        Chunk chunk = new Chunk(chunkX, chunkZ, (int) worldHeight);
        int sectionCount = chunk.Sections.Length;

        for (int sectionY = 0; sectionY < sectionCount; sectionY++)
        {
            ChunkSection section = chunk.Sections[sectionY];
            
            for (int y = 0; y < 16; y++)
            for (int z = 0; z < 16; z++)
            for (int x = 0; x < 16; x++)
            {
                int globalY = sectionY * 16 + y;

                int index = (y * 16 + z) * 16 + x;

                int blockState = GetBlockState(globalY, (int) worldHeight - 64);

                if (globalY == 0)
                    blockState = 7; // bedrock
                else if (globalY < 4)
                    blockState = 1; // stone
                else if (globalY == 4)
                    blockState = 2; // grass block
                else
                    blockState = 0; // air

                section.Blocks.Set(index, blockState);

                if (blockState != 0)
                    section.BlockCount++;
            }
        }

        BuildLighting(chunk);

        return chunk;
    }

    private int GetBlockState(int curHeight, int maxHeight)
    {
        // This little function will represent a crappy replica of Earth when generating blocks
        float percentage = ExtendedMath.Percentage(0, maxHeight, curHeight);
        percentage = Math.Clamp(percentage, 0f, 1f);

        if (percentage <= 1250f / 6520f)
        {
            // Lava
            return BlockManager.GetBlockId(Identifier.Parse("minecraft:lava"));
        }

        if (percentage <= 2200f / 6520f)
        {
            // Slate
            return BlockManager.GetBlockId(Identifier.Parse("minecraft:deepslate"));
        }

        if (percentage <= 2900f / 6520f)
        {
            // Stone
            return BlockManager.GetBlockId(Identifier.Parse("minecraft:stone"));
        }

        if (percentage <= 2971f / 6520f)
        {
            // Grass
            return BlockManager.GetBlockId(Identifier.Parse("minecraft:grass_block"));
        }
        
        // Air
        return BlockManager.GetBlockId(Identifier.Parse("minecraft:air"));
    }

    private void BuildLighting(Chunk chunk)
    {
        int sectionCount = chunk.Sections.Length;
        int lightSections = sectionCount + 2;

        for (int i = 0; i < lightSections; i++)
        {
            chunk.Lightmap.SkyLightMask.Set(i);

            byte[] sky = new byte[2048];
            Array.Fill(sky, (byte)0xFF);
            chunk.Lightmap.SkyLightArrays.Add(sky);
            
            chunk.Lightmap.EmptyBlockLightMask.Set(i);
        }
    }

    private int? GetWorldHeight(Identifier world)
    {
        var dimensionRegistry = RegistryManager.GetRegistry(Identifier.Parse("minecraft:dimension_type"));

        if (dimensionRegistry == null)
            return null;

        var worldEntry = RegistryManager.GetEntryFromRegistry(dimensionRegistry, world);

        if (worldEntry == null)
            return null;

        if (worldEntry.Data == null)
            return null;

        if (worldEntry.Data.TryGet<NbtInt>("height", out var heightTag))
        {
            if (heightTag == null)
            {
                return null;
            }
            
            int height = heightTag.Value;
            return height;
        }

        return null;
    }
}