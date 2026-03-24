using API.Protocol.Packets;

namespace API.Protocol.Types;

public class PrefixedOptional<T> : IWriteToPackets
    where T : IWriteToPackets
{
    private readonly T? _value;

    private bool _shouldWrite;

    public PrefixedOptional(T value, bool sw = true)
    {
        CheckValidity();
        
        _value = value;
        _shouldWrite = sw;
    }

    private void CheckValidity()
    {
        if (typeof(T) != typeof(IWriteToPackets))
            throw new InvalidOperationException("Type does not inherit IWriteToPackets interface!");

        // I mean if we're writing an optional of nothing... we really shouldn't write anything.
        if (_value == null)
            _shouldWrite = false;
    }
    
    public void WriteToPacket(Packet packet)
    {
        if (_shouldWrite)
        {
            packet.Write(true);
            _value!.WriteToPacket(packet);
        }

        else
        {
            packet.Write(false);
            packet.Write(0);
        }
        
    }
}