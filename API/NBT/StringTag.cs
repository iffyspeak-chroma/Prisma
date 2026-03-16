using System.Text;

namespace API.NBT;

public class StringTag : AbstractTagType
{
    public override byte TagType => 8;

    public StringTag(string? name, string value, bool inList)
    {
        TagName = name;
        ValidateName();
        
        if (value.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Tag value is too long (Greater than {ushort.MaxValue})", value);
        }
        
        AddIdentifiers(!inList);
        
        Payload.AddRange(BitConverter.GetBytes((ushort) value.Length));
        Payload.AddRange(Encoding.UTF8.GetBytes(value));
    }
}