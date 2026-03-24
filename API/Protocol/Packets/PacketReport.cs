using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Protocol.Packets;

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
        public Dictionary<string, PacketInformation> Clientbound { get; set; }

        [JsonPropertyName("serverbound")]
        public Dictionary<string, PacketInformation> Serverbound { get; set; }
    }

    public class PacketInformation
    {
        [JsonPropertyName("protocol_id")]
        public int Id { get; set; }
    }

    public void Load(string path)
    {
        string json = File.ReadAllText(path);
        Mapping = JsonSerializer.Deserialize<MappingRoot>(json);
    }
}