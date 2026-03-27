using System.Text.Json.Serialization;

namespace API.Protocol.Status.Components;

public class StatusVersionComponent
{
    [JsonPropertyName("name")] public string VersionName { get; set; } = "Prisma 26.1";

    [JsonPropertyName("protocol")] public int VersionNumber { get; set; } = 775;
}