using API.Protocol.Packets;

namespace API.Protocol.Types;

public class Optional<T> : ISerializable
    where T : ISerializable
{
    private readonly T? _value;

    private bool _shouldWrite;

    public Optional(T value, bool sw = true)
    {
        CheckValidity();
        
        _value = value;
        _shouldWrite = sw;
    }

    private void CheckValidity()
    {
        if (typeof(T) != typeof(ISerializable))
            throw new InvalidOperationException("Type does not inherit IWriteToPackets interface!");

        // I mean if we're writing an optional of nothing... we really shouldn't write anything.
        if (_value == null)
            _shouldWrite = false;
    }

    public void Serialize(Packet packet)
    {
        if (_shouldWrite)
            _value!.Serialize(packet);
        else
            packet.Write(0);
    }
}