using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationPluginMessagePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:custom_payload"].Id);
            packet.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

            return;
        }
        
        packet.Write("minecraft:brand"); // Identifier
        packet.Write("Prisma"); // Data
            
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:custom_payload"].Id);
        packet.WriteLength();
            
        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

        await new ClientboundConfigurationFeatureFlagsPacket().Call(context, null);
    }
}