using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayChatPacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return Task.CompletedTask;
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        string message = packet.ReadString();
        // TODO: Theres more to this packet than just the message
        LogTool.Info($"[CHAT] {player.GetPlayerIdentifier()} : {message}");
        
        // TODO: Send the chat to all other players.
        return Task.CompletedTask;
    }
}