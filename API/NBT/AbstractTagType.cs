using System.Text;

namespace API.NBT;

public abstract class AbstractTagType
{
    public abstract byte TagType { get; }
    public string? TagName { get; set; }
    public List<byte> Payload { get; set; } = new();

    public void ValidateName()
    {
        if (this.TagName != null && 
            this.TagName.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Tag name is too long (Greater than {ushort.MaxValue})", TagName);
        }
    }

    public void AddIdentifiers(bool addType)
    {
        if (addType)
        {
            Payload.Add(TagType);
        }

        if (TagName != null)
        {
            Payload.AddRange(BitConverter.GetBytes((ushort) TagName.Length));
            Payload.AddRange(Encoding.UTF8.GetBytes(TagName));
            
        }
    }
}