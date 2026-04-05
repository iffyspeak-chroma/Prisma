using API.Core;

namespace API.Game.World.Chunk;

public class Heightmap
{
    private readonly int bitsPerEntry;
    private readonly long[] data;
    public HeightmapType Type;

    public Heightmap(int height, HeightmapType type)
    {
        bitsPerEntry = ExtendedMath.CeilLog2(height + 1);
        int totalBits = 256 * bitsPerEntry;
        int longCount = (totalBits + 63) / 64;
        data = new long[longCount];
        Type = type;
    }

    public int Get(int index)
    {
        int bitIndex = index * bitsPerEntry;
        int startLong = bitIndex / 64;
        int startOffset = bitIndex % 64;

        long value = data[startLong] >> startOffset;
        int bitsLeft = 64 - startOffset;

        if (bitsLeft < bitsPerEntry)
        {
            value |= data[startLong + 1] << bitsLeft;
        }

        return (int)(value & ((1L << bitsPerEntry) - 1));
    }
    
    public void Set(int index, int value)
    {
        int bitIndex = index * bitsPerEntry;
        int startLong = bitIndex / 64;
        int startOffset = bitIndex % 64;

        long mask = ((1L << bitsPerEntry) - 1L) << startOffset;

        data[startLong] = (data[startLong] & ~mask) | (((long)value << startOffset) & mask);

        int bitsLeft = 64 - startOffset;

        if (bitsLeft < bitsPerEntry)
        {
            int remainingBits = bitsPerEntry - bitsLeft;
            long mask2 = (1L << remainingBits) - 1L;
            data[startLong + 1] = (data[startLong + 1] & ~mask2) | ((long)value >> bitsLeft);
        }
    }

    public long[] GetRaw() => data;
}