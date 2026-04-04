using API.Protocol.Packets;

namespace API.Core.Transform;

public class Heading : ISerializable
{
    public float Yaw;
    public float Pitch;

    public Heading()
    {
        Yaw = 0;
        Pitch = 0;
    }
    
    public Heading(float yaw, float pitch)
    {
        Yaw = yaw;
        Pitch = pitch;
    }

    public void Serialize(Packet packet)
    {
        packet.Write(Yaw);
        packet.Write(Pitch);
    }
}