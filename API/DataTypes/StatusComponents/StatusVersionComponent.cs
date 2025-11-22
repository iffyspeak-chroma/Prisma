using System.Text.Json.Serialization;

namespace API.DataTypes.StatusComponents;

public class StatusVersionComponent
{
    [JsonPropertyName("name")] public string VersionName { get; set; } = "Prisma 1.21.9";

    [JsonPropertyName("protocol")] public int VersionNumber { get; set; } = 773;
}