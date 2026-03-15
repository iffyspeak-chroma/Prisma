namespace API.NBT;

public class ByteArrayTag : AbstractTagType
{
    public override byte TagType => 7;

    public ByteArrayTag(string name, byte[] value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        AddToPayload(value);
    }

    public ByteArrayTag(string name, List<byte> value)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers();
        
        AddToPayload(value.ToArray());
    }

    private void AddToPayload(byte[] v)
    {
        Payload.AddRange(BitConverter.GetBytes(v.Length));
        Payload.AddRange(v);
    }
}