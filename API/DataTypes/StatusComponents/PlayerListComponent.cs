using System.Text.Json.Serialization;

namespace API.DataTypes.StatusComponents;

public class PlayerListComponent
{
    [JsonPropertyName("max")] 
    public int MaxPlayerCount { get; set; } = 20;

    [JsonPropertyName("online")] 
    public int OnlinePlayerCount { get; set; } = 0;
        
    [JsonPropertyName("sample")]
    public List<PlayerInfoComponent> PlayerSample { get; set; } = new List<PlayerInfoComponent>();
}