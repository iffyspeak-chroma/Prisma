using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Packets.GamestateIndependent.Clientbound;
using Server.Tools;

namespace Server.Managers;

public class PlayerManager
{
    public static readonly PlayerManager Instance = new PlayerManager();

    public static async Task DisconnectPlayer(NetworkedClient player, TextComponentBuilder reason)
    {
        ClientboundDisconnectPacket packet = new ClientboundDisconnectPacket();
        packet.DisconnectMessage = reason;
        
        await packet.Call(player, new Packet());
    }
    
    public static async Task DisconnectPlayer(NetworkedClient player, string reason = "No reason specified!")
    {
        TextComponentBuilder builder = new TextComponentBuilder();

        builder.AddText("Disconnected!\n", color: Constants.ErrorColorPrimary, bold: true);
        builder.AddText($"{reason}", color: Constants.ErrorColorSecondary);

        await DisconnectPlayer(player, builder);
    }
    
    public static async Task SendHeartbeat(NetworkedClient player)
    {
        await new ClientboundKeepAlivePacket().Call(player, null);
    }

    public static async Task SendPacketToAll(Packet packet)
    {
        foreach (KeyValuePair<IChannel, NetworkedClient> pair in Instance.ConnectedClients)
        {
            await pair.Value.SendPacket(packet);
        }
    }

    public static async Task SendPacketToAll(Packet packet, NetworkedClient exception)
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