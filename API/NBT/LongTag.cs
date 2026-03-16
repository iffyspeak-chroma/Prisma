namespace API.NBT;

public class LongTag : AbstractTagType
{
    public override byte TagType => 4;

    public LongTag(string? name, long value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}