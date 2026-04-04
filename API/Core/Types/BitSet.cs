using API.Protocol.Packets;

namespace API.Core.Types;

public class BitSet : ISerializable
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

    public void Serialize(Packet packet)
    {
        packet.Write(data.Count);
        foreach (long l in data)
        {
            packet.Write(l, asVarLong: false);
        }
    }
}