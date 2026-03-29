using API.Protocol.Mojang;

namespace API.DataPacks;

public class RegistryEntry
{
    public Identifier Id { get; init; }
    public string RawJson { get; init; }
}