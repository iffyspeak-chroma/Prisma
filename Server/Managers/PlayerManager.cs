using System.Diagnostics;
using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Packets.GamestateIndependent.Clientbound;

namespace Server.Managers;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new PlayerManager();

    public static void DisconnectPlayer(NetworkedClient player, TextComponentBuilder reason)
    {
        ClientboundDisconnectPacket packet = new ClientboundDisconnectPacket();
        packet.DisconnectMessage = reason;
        
        packet.Call(player, new Packet());
    }
    
    public static void DisconnectPlayer(NetworkedClient player, string reason = "No reason specified!")
    {
        TextComponentBuilder builder = new TextComponentBuilder();

        builder.AddText("Disconnected!\n", color: "white", bold: true);
        builder.AddText($"{reason}", color: "red");

        DisconnectPlayer(player, builder);
    }

    public static async void SendPacketToAll(Packet packet)
    {
        foreach (KeyValuePair<IChannel, NetworkedClient> pair in Instance.ConnectedClients)
        {
            await pair.Value.SendPacket(packet);
        }
    }

    public static async void SendPacketToAll(Packet packet, NetworkedClient exception)
    {
        foreach (KeyValuePair<IChannel, NetworkedClient> pair in Instance.ConnectedClients)
        {
            if (pair.Value != exception)
            {
                await pair.Value.SendPacket(packet);
            }
        }
    }
    
    public Dictionary<IChannel, NetworkedClient> ConnectedClients = new Dictionary<IChannel, NetworkedClient>();
}