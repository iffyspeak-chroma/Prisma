using API.Protocol.Mojang;

namespace API.DataPacks;

public class TagGroup
{
    public Identifier RegistryId { get; init; }
    public Dictionary<Identifier, Tag> Tags { get; } = new();
}