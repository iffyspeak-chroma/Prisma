using System.Text;

namespace API.NBT;

public class ByteTag : AbstractTagType
{
    public override byte TagType => 1;
    public override string? TagName { get; }
    public override List<byte> Payload { get; set; }

    public ByteTag(string name, byte value)
    {
        ValidateName();
        
        AddIdentifiers(name);
        
        Payload.Add(value);
    }
}