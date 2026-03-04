using API.Networking;

namespace API.DataTypes.Mojang;

public class Tag : IWriteToPackets
{
    public Identifier TagName { get; private set; }
    public List<int> Entries { get; private set; }

    public Tag(Identifier name, List<int> entries)
    {
        this.TagName = name;
        this.Entries = entries;
    }
    
    public void Write(Packet packet)
    {
        TagName.Write(packet);
        
        packet.Write(Entries.Count);
        foreach (int entry in Entries)
        {
            packet.Write(entry);
        }
    }
}