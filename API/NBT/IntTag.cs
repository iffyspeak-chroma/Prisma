namespace API.NBT;

public class IntTag : AbstractTagType
{
    public override byte TagType => 3;

    public IntTag(string name, int value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();

        Payload.AddRange(BitConverter.GetBytes(value));
    }
}