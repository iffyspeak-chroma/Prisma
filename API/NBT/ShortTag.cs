namespace API.NBT;

// It's like tag but for short people
public class ShortTag : AbstractTagType
{
    public override byte TagType => 2;
    public override string? TagName { get; }
    public override List<byte> Payload { get; set; }

    public ShortTag(string name, short value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}