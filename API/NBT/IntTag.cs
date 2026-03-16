namespace API.NBT;

public class IntTag : AbstractTagType
{
    public override byte TagType => 3;

    public IntTag(string? name, int value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);

        Payload.AddRange(BitConverter.GetBytes(value));
    }
}