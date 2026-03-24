using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Status.Clientbound;

public class ClientboundStatusResponsePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet(packet.ToArray()))
        {
            p.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:status_response"].Id);
            p.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}