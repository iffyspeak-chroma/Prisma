using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Managers;
using Server.Tools;

namespace Server.Packets.GamestateIndependent.Serverbound;

public class ServerboundKeepAlivePacket : ICallable
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return;
        
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        long rxTimestamp = packet.ReadLong();
        long last = client.LastHeartbeat;

        // Disconnect the player if the timestamp id they send is 5 seconds out of date.
        if (!(last - rxTimestamp >= 5000))
        {
            client.ResetTimeout();
            return;
        }
        
        TextComponentBuilder builder = new TextComponentBuilder();
        builder.AddText("Disconnected!", color: Constants.ErrorColorPrimary, bold: true);
        builder.AddText("Client is too desynced from server.", color: Constants.ErrorColorSecondary);

        await PlayerManager.DisconnectPlayer(client, builder);
    }
}