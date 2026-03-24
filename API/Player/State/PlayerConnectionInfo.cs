namespace API.Player.State;

public class PlayerConnectionInfo
{
    public int ProtocolVersion { get; set; }
    public string ServerAddress { get; set; }
    public ushort ServerPort { get; set; }

    public PlayerConnectionInfo()
    {
        this.ProtocolVersion = 0;
        this.ServerAddress = "";
        this.ServerPort = 0;
    }

    public PlayerConnectionInfo(int protocolVersion, string serverAddress, ushort serverPort)
    {
        this.ProtocolVersion = protocolVersion;
        this.ServerAddress = serverAddress;
        this.ServerPort = serverPort;
    }

    public PlayerConnectionInfo(string serverAddress, ushort serverPort)
    {
        this.ProtocolVersion = 0;
        this.ServerAddress = serverAddress;
        this.ServerPort = serverPort;
    }

}