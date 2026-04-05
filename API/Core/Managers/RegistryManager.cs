using API.DataPacks;
using API.Protocol.Mojang;

namespace API.Core.Managers;

public class RegistryManager
{
    public static RegistryManager Instance = new RegistryManager();

    public List<Registry> Registries { get; } = new();
    public List<TagEntry> Tags { get; } = new();

    public static Registry? GetRegistry(Identifier id)
    {
        return Instance.Registries.FirstOrDefault(r => r.RegistryId.ToString() == id.ToString());
    }

    public static RegistryEntry? GetEntryFromRegistry(Registry registry, Identifier id)
    {
        return registry.Entries.FirstOrDefault(e => e.EntryId.ToString() == id.ToString());
    }
}