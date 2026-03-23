using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Play.Clientbound;
using Server.Managers;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationAcknowledgeFinishPacket : ICallable
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        client.Gamestate = PlayerGamestate.Play;
        LogTool.Info($"{player.GetPlayerIdentifier()} finished configuration!");
        
        await new ClientboundPlayLoginPacket().Call(context, null);
    }
}