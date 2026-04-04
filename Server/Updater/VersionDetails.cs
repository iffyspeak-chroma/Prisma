using System.Text.Json.Serialization;

namespace Server.Updater;

public class VersionDetails
{
    [JsonPropertyName("downloads")] public VersionDownloads Downloads { get; set; }
}