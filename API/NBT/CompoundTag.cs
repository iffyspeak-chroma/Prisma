namespace API.NBT;

public class CompoundTag : AbstractTagType
{
    public override byte TagType => 10;
    public List<AbstractTagType> Children = new();
    public bool IsRoot;

    public CompoundTag(string? name, bool isRoot)
    {
        TagName = name;
        IsRoot = isRoot;
        ValidateName();

        AddIdentifiers(true);
    }

    public void AssemblePayload()
    {
        foreach (AbstractTagType child in Children)
        {
            Payload.AddRange(child.Payload);
        }

        if (!IsRoot)
        {
            Payload.Add(new EndTag().TagType);
        }
    }
}