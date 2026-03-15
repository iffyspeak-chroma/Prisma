namespace API.NBT;

public abstract class AbstractTagType
{
    public abstract byte TagType { get; }
    public abstract string? TagName { get; }
    public abstract List<byte> Payload { get; set; }
}