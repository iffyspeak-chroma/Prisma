using API.Protocol.Networking;

namespace API.Game.Events;

public class PlayerDisconnectEvent(NetworkedClient client, DisconnectReason reason) : EventArgs
{
    public NetworkedClient Client { get; } = client;
    public DisconnectReason Reason { get; } = reason;
}

