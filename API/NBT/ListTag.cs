namespace API.NBT;

public class ListTag<TType> : AbstractTagType where TType : AbstractTagType
{
    public override byte TagType => 9;

    public ListTag(string? name, List<TType> tags, bool inList = false)
    {
        TagName = name;
        ValidateName();
        
        AddIdentifiers(!inList);

        if (tags.Count <= 0)
        {
            // Use the EndTag
            Payload.Add(new EndTag().TagType);
            return;
        }
        
        Payload.Add(tags[0].TagType);
        
        Payload.Add((byte)((tags.Count >> 24) & 0xFF));
        Payload.Add((byte)((tags.Count >> 16) & 0xFF));
        Payload.Add((byte)((tags.Count >> 8) & 0xFF));
        Payload.Add((byte)(tags.Count & 0xFF));
        foreach (var tag in tags)
        {
            Payload.AddRange(tag.Payload);
        }
    }
    
}