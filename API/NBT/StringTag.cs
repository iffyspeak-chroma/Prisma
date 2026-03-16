using System.Text;

namespace API.NBT;

public class StringTag : AbstractTagType
{
    public override byte TagType => 8;

    public StringTag(string name, string value)
    {
        TagName = name;
        ValidateName();

        var strBytes = Encoding.UTF8.GetBytes(value);

        // Write length as 2-byte big-endian
        Payload.Add((byte)((strBytes.Length >> 8) & 0xFF));
        Payload.Add((byte)(strBytes.Length & 0xFF));

        // Write string bytes
        Payload.AddRange(strBytes);
    }
}