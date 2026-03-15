namespace API.NBT;

public class DoubleTag : AbstractTagType
{
    public override byte TagType => 6;

    public DoubleTag(string name, double value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}