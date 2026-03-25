using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFinishPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:finish_configuration"].Id);
            packet.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
            
            return;
        }
        
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:finish_configuration"].Id);
        packet.WriteLength();

        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
    }
}