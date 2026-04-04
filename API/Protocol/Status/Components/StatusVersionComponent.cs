using System.Text.Json.Serialization;
using API.Core;

namespace API.Protocol.Status.Components;

public class StatusVersionComponent
{
    [JsonPropertyName("name")] public string VersionName { get; set; } = $"Prisma {Server.Instance?.VersionName}";

    [JsonPropertyName("protocol")] public int VersionNumber { get; set; } = Server.Instance.ProtocolId;
}