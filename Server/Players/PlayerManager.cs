using System.Diagnostics;
using API.DataTypes.Player;
using DotNetty.Transport.Channels;

namespace Server.Players;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new  PlayerManager();
    
    public Dictionary<IChannel, NetworkedClient> ConnectedClients = new Dictionary<IChannel, NetworkedClient>();
}