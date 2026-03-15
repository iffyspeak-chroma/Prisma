using API.Logging;

namespace API.NBT;

public class EndTag : AbstractTagType
{
    public override byte TagType => 0;
    public override string? TagName => null;
    public override List<byte> Payload { get; set; }

    public EndTag()
    {
        // Have this tag for completion’s sake
        return;
    }
}