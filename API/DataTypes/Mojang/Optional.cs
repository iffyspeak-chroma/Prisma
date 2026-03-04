using API.Networking;

namespace API.DataTypes.Mojang;

public class Optional<T> : IWriteToPackets
    where T : IWriteToPackets
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
        if (typeof(T) != typeof(IWriteToPackets))
            throw new InvalidOperationException("Type does not inherit IWriteToPackets interface!");

        // I mean if we're writing an optional of nothing... we really shouldn't write anything.
        if (_value == null)
            _shouldWrite = false;
    }

    public void WriteToPacket(Packet packet)
    {
        if (_shouldWrite)
            _value!.WriteToPacket(packet);
        else
            packet.Write(0);
    }
}