using System.Text;

namespace API.NBT;

public class ByteTag : AbstractTagType
{
    public override byte TagType => 1;

    public ByteTag(string name, byte value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.Add(value);
    }
}