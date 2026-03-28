using System.Text.Json.Serialization;

namespace Server.Updater;

public class VersionManifest
{
    [JsonPropertyName("latest")] public ManifestLatestVersions Latest { get; set; }
    [JsonPropertyName("versions")] public ManifestVersionData[] Versions { get; set; }
}