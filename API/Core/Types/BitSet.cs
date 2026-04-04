namespace API.Core.Types;

public class BitSet
{
    private readonly List<long> data = new();

    public void Set(int index)
    {
        int longIndex = index / 64;

        while (data.Count <= longIndex)
        {
            data.Add(0);
        }

        data[longIndex] |= 1L << (index % 64);
    }

    public bool Get(int index)
    {
        int longIndex = index / 64;
        if (longIndex >= data.Count) return false;

        return (data[longIndex] & (1L << (index % 64))) != 0;
    }
}