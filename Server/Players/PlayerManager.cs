using System.Diagnostics;
using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Packets.GamestateIndependent.Clientbound;

namespace Server.Players;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new  PlayerManager();

    public static void DisconnectPlayer(NetworkedClient player, TextComponentBuilder reason)
    {
        ClientboundDisconnectPacket packet = new ClientboundDisconnectPacket();
        packet.DisconnectMessage = reason;
        
        packet.Call(player, new Packet());
    }
    
    public static void DisconnectPlayer(NetworkedClient player, string reason)
    {

        TextComponentBuilder builder = new TextComponentBuilder();

        builder.AddText("Uh oh!\n", color: "red");
        builder.AddText($"{reason}");

        DisconnectPlayer(player, builder);
    }
    
    public Dictionary<IChannel, NetworkedClient> ConnectedClients = new Dictionary<IChannel, NetworkedClient>();
}