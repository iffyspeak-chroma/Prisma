using System.Text.Json.Serialization;

namespace Server.Updater;

public class ManifestLatestVersions
{
    [JsonPropertyName("release")] public string Release { get; set; }
    [JsonPropertyName("snapshot")] public string Snapshot { get; set; }
}