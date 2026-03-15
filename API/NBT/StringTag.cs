using System.Text;

namespace API.NBT;

public class StringTag : AbstractTagType
{
    public override byte TagType => 8;
    public override string? TagName { get; } = "";
    public override List<byte> Payload { get; set; } = new();

    public StringTag(string name, string value)
    {
        if (name.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Tag name is too long (Greater than {ushort.MaxValue})", name);
        }

        if (value.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Tag value is too long (Greater than {ushort.MaxValue})", value);
        }
        
        Payload.Add(TagType);
        
        Payload.AddRange(BitConverter.GetBytes((ushort) name.Length));
        Payload.AddRange(Encoding.UTF8.GetBytes(name));
        
        Payload.AddRange(BitConverter.GetBytes((ushort) value.Length));
        Payload.AddRange(Encoding.UTF8.GetBytes(value));
    }
}