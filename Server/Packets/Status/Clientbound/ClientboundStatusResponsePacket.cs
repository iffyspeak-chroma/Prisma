using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Status.Clientbound;

public class ClientboundStatusResponsePacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet(packet.ToArray()))
        {
            p.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:status_response"].Id);
            p.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}