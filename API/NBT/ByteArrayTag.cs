namespace API.NBT;

public class ByteArrayTag : AbstractTagType
{
    public override byte TagType => 7;

    public ByteArrayTag(string? name, byte[] value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        AddToPayload(value);
    }

    public ByteArrayTag(string? name, List<byte> value, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);
        
        AddToPayload(value.ToArray());
    }

    private void AddToPayload(byte[] v)
    {
        Payload.AddRange(BitConverter.GetBytes(v.Length));
        Payload.AddRange(v);
    }
}