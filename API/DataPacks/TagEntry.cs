using API.Protocol.Mojang;

namespace API.DataPacks;

public class TagEntry
{
    public Identifier RegistryId { get; init; }
    public Identifier TagName { get; init; }
    public List<int> Entries { get; set; } = new();
}