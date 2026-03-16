namespace API.NBT;

public class DoubleTag : AbstractTagType
{
    public override byte TagType => 6;

    public DoubleTag(string? name, double value, bool inList)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}