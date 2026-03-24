using API.Protocol.Packets;

namespace API.Core.Transform;

public class Location : IWriteToPackets
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Location()
    {
        X = 0d;
        Y = 0d;
        Z = 0d;
    }

    public Location(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public void WriteToPacket(Packet packet)
    {
        packet.Write(X);
        packet.Write(Y);
        packet.Write(Z);
    }
}