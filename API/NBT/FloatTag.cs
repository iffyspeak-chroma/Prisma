namespace API.NBT;

public class FloatTag : AbstractTagType
{
    public override byte TagType => 5;
    
    public FloatTag(string? name, float value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}