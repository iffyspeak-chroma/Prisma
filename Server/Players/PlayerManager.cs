using API.DataTypes.Player;
using DotNetty.Transport.Channels;

namespace Server.Players;

public class PlayerManager
{
    public PlayerManager Instance = new  PlayerManager();
    
    public Dictionary<IChannel, NetworkedClient> ConnectedClients = new Dictionary<IChannel, NetworkedClient>();
}