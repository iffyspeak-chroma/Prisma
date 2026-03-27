using System.Text.Json.Serialization;

namespace API.Protocol.Status.Components;

public class PlayerInfoComponent
{
    [JsonPropertyName("name")] public string PlayerName { get; set; } = "null";
        
    [JsonPropertyName("id")] public string UUID { get; set; } = "00000000-0000-0000-0000-000000000000";
}