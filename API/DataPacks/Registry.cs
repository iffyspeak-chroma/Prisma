using API.Protocol.Mojang;

namespace API.DataPacks;

public class Registry
{
    public Identifier Identifier { get; init; }
    public Dictionary<Identifier, RegistryEntry> Entries { get; } = new();
}