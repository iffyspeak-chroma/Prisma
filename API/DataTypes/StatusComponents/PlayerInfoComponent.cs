using System.Text.Json.Serialization;

namespace API.DataTypes.StatusComponents;

public class PlayerInfoComponent
{
    [JsonPropertyName("name")]
    public string PlayerName { get; set; }
        
    [JsonPropertyName("id")]
    public string UUID { get; set; }
}