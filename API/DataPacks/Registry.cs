using API.Protocol.Mojang;

namespace API.DataPacks;

public class Registry
{
    public Identifier RegistryId { get; init; }
    public List<RegistryEntry> Entries { get; } = new();
}