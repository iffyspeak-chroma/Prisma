using API.Logging;
using API.Player;
using API.Player.State;
using API.Protocol.Networking;
using API.Protocol.Packets;
using API.Text;
using DotNetty.Transport.Channels;
using Server.Packets.Play.Clientbound;
using Server.Managers;
using Server.Tools;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationAcknowledgeFinishPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        client.Gamestate = PlayerGamestate.Play;
        LogTool.Info($"{player.GetPlayerIdentifier()} finished configuration!");

        client.TimeoutReached += HandleTimeout;
        client.StartTimeoutCountdown();
        
        await new ClientboundPlayLoginPacket().Call(context, null);
    }

    private void HandleTimeout(NetworkedClient client)
    {
        TextComponentBuilder builder = new TextComponentBuilder();
        builder.AddText("Disconnected!", color: Constants.ErrorColorPrimary, bold: true);
        builder.AddText("Client timed out.", color: Constants.ErrorColorSecondary);

        _ = PlayerManager.DisconnectPlayer(client, builder);
    }
}