using System.Text;

namespace API.NBT;

public class ByteTag : AbstractTagType
{
    public override byte TagType => 1;

    public ByteTag(string? name, byte value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        Payload.Add(value);
    }
}