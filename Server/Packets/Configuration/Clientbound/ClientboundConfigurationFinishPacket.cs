using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFinishPacket : ICallable
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