namespace API.Core;

public class ServerConfiguration
{
    public string BindAddress { get; set; } = "0.0.0.0";
    public ushort Port { get; set; } = 26656;
    public bool DebugMode { get; set; } = false;
    public bool DoEncryption { get; set; } = true;
}