using API.DataTypes.Player;

namespace Server.Players;

public class PlayerManager
{
    public PlayerManager Instance = new  PlayerManager();
    
    public List<NetworkedClient> ConnectedClients = new List<NetworkedClient>();
}