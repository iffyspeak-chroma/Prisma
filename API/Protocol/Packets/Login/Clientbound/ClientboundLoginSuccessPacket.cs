using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Login.Clientbound;

public class ClientboundLoginSuccessPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet(packet.ToArray()))
        {
            p.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_finished"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}