using API.Core;
using API.Core.Managers;

namespace API.Game.World.Chunk;

public class PalettedContainer
{
    private byte bitsPerEntry;
    private readonly List<int> palette = new();
    private long[] data;

    private readonly int size;

    public PalettedContainer(int size)
    {
        this.size = size;
        palette.Add(0);
        bitsPerEntry = 1;
        ResizeData();
    }
    
    private void ResizeData()
    {
        int totalBits = size * bitsPerEntry;
        int longCount = (totalBits + 63) / 64;
        data = new long[longCount];
    }
    
    private int GetPaletteIndex(int value)
    {
        int index = palette.IndexOf(value);
        if (index != -1) return index;

        palette.Add(value);
        byte neededBits = (byte) ExtendedMath.CeilLog2(palette.Count);

        if (neededBits > bitsPerEntry)
        {
            Resize(neededBits);
        }

        return palette.Count - 1;
    }
    
    private void Resize(byte newBits)
    {
        var oldData = data;
        int oldBits = bitsPerEntry;

        bitsPerEntry = newBits;
        ResizeData();

        for (int i = 0; i < size; i++)
        {
            int val = GetRaw(i, oldData, oldBits);
            SetRaw(i, val);
        }
    }
    
    private int GetRaw(int index, long[] source, int bits)
    {
        int bitIndex = index * bits;
        int startLong = bitIndex / 64;
        int startOffset = bitIndex % 64;

        long value = source[startLong] >> startOffset;
        int bitsLeft = 64 - startOffset;

        if (bitsLeft < bits)
        {
            value |= source[startLong + 1] << bitsLeft;
        }

        return (int)(value & ((1L << bits) - 1));
    }

    private void SetRaw(int index, int value)
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
    
    public void Set(int index, int blockState)
    {
        int paletteIndex = GetPaletteIndex(blockState);
        SetRaw(index, paletteIndex);
    }

    public int Get(int index)
    {
        int paletteIndex = GetRaw(index, data, bitsPerEntry);
        return palette[paletteIndex];
    }

    public List<int> GetPalette() => palette;
    public long[] GetData() => data;
    public int BitsPerEntry => bitsPerEntry;
}