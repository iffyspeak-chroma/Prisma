using API.Core.Managers;
using API.Logging;
using API.Player;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Serverbound;

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