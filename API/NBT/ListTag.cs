namespace API.NBT;

public class ListTag<TType> : AbstractTagType where TType : AbstractTagType
{
    public override byte TagType => 9;

    public ListTag(string? name, List<TType> tags, bool inList)
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
        
        Payload.AddRange(BitConverter.GetBytes(tags.Count));
        foreach (var tag in tags)
        {
            Payload.AddRange(tag.Payload);
        }
    }
    
}