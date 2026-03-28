using System.Text.Json.Serialization;

namespace Server.Updater;

public class VersionDownloads
{
    [JsonPropertyName("client")] public VersionDownloadInfo Client { get; set; }
    [JsonPropertyName("server")] public VersionDownloadInfo Server { get; set; }
}