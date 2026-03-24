using API.Logging;
using API.Player;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayPlayerLoadedPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        LogTool.Info($"{player.GetPlayerIdentifier()} successfully connected!");
        
        // TODO: I'm not sure if anything else needs to happen here, potentially a chat message?
        return Task.CompletedTask;
    }
}