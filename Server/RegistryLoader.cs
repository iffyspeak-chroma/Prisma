using System.Text.Json;
using API.Core.Managers;
using API.DataPacks;
using API.Logging;
using API.Protocol.Mojang;
using API.Protocol.NBT;
using fNbt;

namespace Server;

public class RegistryLoader
{
    public RegistryLoader()
    {
        LoadRegistryIds();
        LoadRegistryData();
    }
    
    void LoadRegistryIds()
    {
        var json = File.ReadAllText(Constants.RegistriesReportFile);

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var root = JsonSerializer.Deserialize<RootDto>(json, options);

        foreach (var (registryKey, registryDto) in root!)
        {
            var registry = new Registry
            {
                RegistryId = Identifier.Parse(registryKey)
            };

            foreach (var (entryKey, entryJson) in registryDto.Entries)
            {
                var tag = NbtToolkit.ParseElement(entryJson);

                var compound = tag as NbtCompound ?? new NbtCompound("");
                
                registry.Entries.Add(new RegistryEntry
                {
                    EntryId = Identifier.Parse(entryKey),
                    Data = compound
                });
            }

            RegistryManager.Instance.Registries.Add(registry);
        }
    }

    void LoadRegistryData()
    {
        var registries = new Dictionary<string, Registry>();
        
        foreach (var namespaceDir in Directory.GetDirectories(Path.Combine(Constants.VanillaDirectory, "data")))
        {
            var ns = Path.GetFileName(namespaceDir);

            foreach (var file in Directory.GetFiles(namespaceDir, "*.json", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(namespaceDir, file).Replace('\\', '/');

                if (relativePath.StartsWith("tags/"))
                    continue;

                var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? "";

                var registryId = string.IsNullOrEmpty(directory) 
                    ? ns.ToLowerInvariant() 
                    : $"{ns}:{directory}".ToLowerInvariant();
                var entryName = Path.GetFileNameWithoutExtension(file);
                var entryId = $"{ns}:{entryName}".ToLowerInvariant();

                var json = File.ReadAllText(file);
                using var doc = JsonDocument.Parse(json);

                var tag = NbtToolkit.ParseElement(doc.RootElement);
                if (!(tag is NbtCompound compound))
                {
                    LogTool.Warn($"Skipping file '{file}' because it is not a JSON object.");
                    continue;
                }

                if (!registries.TryGetValue(registryId, out var registry))
                {
                    registry = new Registry { RegistryId = Identifier.Parse(registryId) };
                    registries.Add(registryId, registry);
                }

                registry.Entries.Add(new RegistryEntry
                {
                    EntryId = Identifier.Parse(entryId),
                    Data = compound
                });
            }
        }

        RegistryManager.Instance.Registries.AddRange(registries.Values);
    }
}