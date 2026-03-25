using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationPluginMessagePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            p.Write("minecraft:brand"); // Identifier
            p.Write("Prisma"); // Data
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:custom_payload"].Id);
            p.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }

        await new ClientboundConfigurationFeatureFlagsPacket().Call(context, null);
    }
}