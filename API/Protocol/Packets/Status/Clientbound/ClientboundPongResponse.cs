using API.Core.Managers;
using API.Logging;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Clientbound;

public class ClientboundPongResponse : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        
        packet.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:pong_response"].Id);
        packet.WriteLength();

        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

        // Since they aren't actually dead-set on playing just yet, we'll remove them from our list
        PlayerManager.Instance.ConnectedClients[context.Channel].DisconnectChannel();
        PlayerManager.Instance.ConnectedClients.Remove(context.Channel);
        
    }
}