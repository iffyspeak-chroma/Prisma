using API.Logging;

namespace API.NBT;

public class EndTag : AbstractTagType
{
    public override byte TagType => 0;

    public EndTag()
    {
        // Have this tag for completion’s sake
        TagName = null;
        return;
    }
}