namespace API.Protocol.Packets;

public interface ISerializable
{
    void Serialize(Packet packet);
}