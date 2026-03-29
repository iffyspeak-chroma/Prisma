using API.Protocol.Mojang;

namespace API.DataPacks;

public class GameData
{
    public Dictionary<Identifier, Registry> Registries { get; } = new();
    public Dictionary<Identifier, TagGroup> TagGroups { get; } = new();
}