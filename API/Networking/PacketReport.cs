using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Networking;

public class PacketReport
{
    public static MappingRoot Mapping { get; private set; }

    public class MappingRoot
    {
        [JsonPropertyName("handshake")]
        public State Handshake { get; set; }

        [JsonPropertyName("login")]
        public State Login { get; set; }

        [JsonPropertyName("status")]
        public State Status { get; set; }
        
        [JsonPropertyName("configuration")]
        public State Configuration { get; set; }
        
        [JsonPropertyName("play")]
        public State Play { get; set; }
    }

    public class State
    {
        [JsonPropertyName("clientbound")]
        public Dictionary<string, Packet> Clientbound { get; set; }

        [JsonPropertyName("serverbound")]
        public Dictionary<string, Packet> Serverbound { get; set; }
    }

    public class Packet
    {
        [JsonPropertyName("protocol_id")]
        public int Id { get; set; }
    }

    public static void Load(string path)
    {
        string json = File.ReadAllText(path);
        Mapping = JsonSerializer.Deserialize<MappingRoot>(json);
    }
}