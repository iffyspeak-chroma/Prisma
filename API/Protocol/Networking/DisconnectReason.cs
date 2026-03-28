namespace API.Protocol.Networking;

public enum DisconnectReason
{
    Unknown,
    Disconnect,
    Exception,
    Generic,
    Kicked,
    TimedOut
}