using System.Text.Json.Serialization;

namespace API.Protocol.Status.Components;

public class StatusVersionComponent
{
    [JsonPropertyName("name")] public string VersionName { get; set; } = "Prisma 1.21.11";

    [JsonPropertyName("protocol")] public int VersionNumber { get; set; } = 774;
}