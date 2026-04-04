using System.Text.Json;
using API.Core.Managers;
using API.DataPacks;
using API.Logging;
using API.Protocol.Mojang;

namespace Server;

public class TagResolver
{
    public TagResolver()
    {
        ResolveAllTags();
    }
    
    Dictionary<string, List<RawTagEntry>> LoadRawTags()
    {
        var result = new Dictionary<string, List<RawTagEntry>>();
        var tagsRoot = Path.Combine(Constants.VanillaDirectory, "data", "minecraft", "tags");

        if (!Directory.Exists(tagsRoot))
            return result;

        foreach (var file in Directory.GetFiles(tagsRoot, "*.json", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(tagsRoot, file).Replace('\\', '/');
            
            var parts = relativePath.Split('/');

            var registryName = parts[0]; 
            var registryId = $"minecraft:{registryName}";
            
            var tagPath = string.Join('/', parts.Skip(1));
            tagPath = tagPath.Substring(0, tagPath.Length - 5);
            var tagId = $"minecraft:{tagPath}";

            using var doc = JsonDocument.Parse(File.ReadAllText(file));

            var rawTag = new RawTagEntry { Id = Identifier.Parse(tagId) };

            if (!doc.RootElement.TryGetProperty("values", out var values))
            {
                LogTool.Warn($"Tag file '{file}' has no 'values' array, skipping.");
                continue;
            }

            foreach (var val in values.EnumerateArray())
                rawTag.Values.Add(val.GetString()!);

            if (!result.TryGetValue(registryId, out var list))
            {
                list = new List<RawTagEntry>();
                result[registryId] = list;
            }

            list.Add(rawTag);
        }

        return result;
    }

    void ResolveAllTags()
    {
        var rawTagsByRegistry = LoadRawTags();

        foreach (var (registryId, rawTags) in rawTagsByRegistry)
        {
            var registry =
                RegistryManager.Instance.Registries.FirstOrDefault(r =>
                    r.RegistryId.ToString().ToLowerInvariant() == registryId.ToLowerInvariant());

            if (registry == null)
            {
                LogTool.Warn($"Registry '{registryId}' not found, skipping its tags.");
                continue;
            }

            var entryLookup = registry.Entries
                .Where(e => e.Data != null)
                .ToDictionary(
                    e => e.EntryId.ToString().ToLowerInvariant(),
                    e => registry.Entries.IndexOf(e)
                );

            var tagLookup = rawTags.ToDictionary(t => t.Id.ToString());

            foreach (var rawTag in rawTags)
            {
                var resolved = ResolveTagSafe(rawTag, registryId, entryLookup, tagLookup, new HashSet<string>());

                RegistryManager.Instance.Tags.Add(new TagEntry
                {
                    RegistryId = Identifier.Parse(registryId),
                    TagName = rawTag.Id,
                    Entries = resolved.Distinct().ToList()
                });
            }
        }
    }

    List<int> ResolveTagSafe(RawTagEntry tag, string registryId,
        Dictionary<string, int> entryLookup, Dictionary<string, RawTagEntry> tagLookup,
        HashSet<string> visited)
    {
        var result = new List<int>();

        if (!visited.Add(tag.Id.ToString()))
            return result;

        foreach (var value in tag.Values)
        {
            if (value.StartsWith("#"))
            {
                var nestedTagId = value.Substring(1);
                if (tagLookup.TryGetValue(nestedTagId, out var nested))
                {
                    result.AddRange(ResolveTagSafe(nested, registryId, entryLookup, tagLookup, visited));
                }
                continue;
            }
            
            if (entryLookup.TryGetValue(value, out var protocolId))
            {
                result.Add(protocolId);
            }
        }

        return result;
    }
}