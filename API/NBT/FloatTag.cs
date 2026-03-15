namespace API.NBT;

public class FloatTag : AbstractTagType
{
    public override byte TagType => 5;
    
    public FloatTag(string name, float value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        Payload.AddRange(BitConverter.GetBytes(value));
    }
}