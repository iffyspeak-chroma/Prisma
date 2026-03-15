namespace API.NBT;

// It's like tag but for short people
public class ShortTag : AbstractTagType
{
    public override byte TagType => 2;

    public ShortTag(string name, short value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}