using API.DataPacks;

namespace API.Core.Managers;

public class RegistryManager
{
    public static RegistryManager Instance = new RegistryManager();

    public List<Registry> Registries { get; } = new();
    public List<TagEntry> Tags { get; } = new();
}