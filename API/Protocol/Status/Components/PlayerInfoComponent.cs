using System.Text.Json.Serialization;

namespace API.Protocol.Status.Components;

public class PlayerInfoComponent
{
    [JsonPropertyName("name")]
    public string PlayerName { get; set; }
        
    [JsonPropertyName("id")]
    public string UUID { get; set; }
}