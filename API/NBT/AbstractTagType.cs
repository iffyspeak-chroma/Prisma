using System.Text;

namespace API.NBT;

public abstract class AbstractTagType
{
    public abstract byte TagType { get; }
    public abstract string? TagName { get; }
    public abstract List<byte> Payload { get; set; }

    public void ValidateName()
    {
        if (this.TagName != null && 
            this.TagName.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Tag name is too long (Greater than {ushort.MaxValue})", TagName);
        }
    }

    public void AddIdentifiers()
    {
        Payload.Add(TagType);

        if (TagName != null)
        {
            Payload.AddRange(BitConverter.GetBytes((ushort) TagName.Length));
            Payload.AddRange(Encoding.UTF8.GetBytes(TagName));
        }
    }
}