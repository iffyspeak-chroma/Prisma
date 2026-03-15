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
        
        Payload.Add(TagType);
        
        Payload.AddRange(BitConverter.GetBytes((ushort) name.Length));
        Payload.AddRange(Encoding.UTF8.GetBytes(name));
        
        Payload.Add(value);
    }
}