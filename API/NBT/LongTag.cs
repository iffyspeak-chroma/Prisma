namespace API.NBT;

public class LongTag : AbstractTagType
{
    public override byte TagType => 4;

    public LongTag(string name, long value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}