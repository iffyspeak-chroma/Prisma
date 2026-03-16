namespace API.NBT;

public class CompoundTag : AbstractTagType
{
    public override byte TagType => 10;
    public List<AbstractTagType> Children = new();

    public CompoundTag(string? name, bool root)
    {
        TagName = name;
        ValidateName();

        AddIdentifiers(true);

        foreach (AbstractTagType child in Children)
        {
            Payload.AddRange(child.Payload);
        }

        if (!root)
        {
            Payload.Add(new EndTag().TagType);
        }
    }
}