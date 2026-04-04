using System.Text.Json.Serialization;

namespace Server.Updater;

public class ServerVersionInfo
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("protocol_version")] public int ProtocolVersion { get; set; }
    
}