using System.Text.Json.Serialization;

namespace API.Protocol.Status.Components;

public class PlayerListComponent
{
    [JsonPropertyName("max")] 
    public int MaxPlayerCount { get; set; } = 20;

    [JsonPropertyName("online")] 
    public int OnlinePlayerCount { get; set; } = 0;
        
    [JsonPropertyName("sample")]
    public List<PlayerInfoComponent> PlayerSample { get; set; } = new List<PlayerInfoComponent>();
}