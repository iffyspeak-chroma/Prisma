using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationUpdateTagsPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_tags"].Id);
            packet.WriteLength();
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
            return;
        }
        
        packet.Write(RegistryManager.Instance.Tags.Count);

        foreach (var tag in RegistryManager.Instance.Tags)
        {
            tag.TagName.WriteToPacket(packet);
            
            packet.Write(tag.Entries.Count);

            foreach (var protocolId in tag.Entries)
            {
                packet.Write(protocolId);
            }
        }

        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_tags"].Id);
        packet.WriteLength();
        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

        await new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}