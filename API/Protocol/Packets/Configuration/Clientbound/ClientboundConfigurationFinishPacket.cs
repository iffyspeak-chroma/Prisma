using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFinishPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:finish_configuration"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}