using API.Core.Managers;
using API.Protocol.Mojang;
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
        
        Identifier registry = new Identifier("minecraft", "timeline");
        List<Tag> tags = new List<Tag>()
        {
            new Tag(Identifier.Parse("minecraft:in_end"),
                new List<int>()
                {
                    0
                }),

            new Tag(Identifier.Parse("minecraft:in_nether"),
                new List<int>()
                {
                    0
                }),

            new Tag(Identifier.Parse("minecraft:in_overworld"),
                new List<int>()
                {
                    0, 1, 2, 3
                })
        };

        // Only writing to 1 registry? (Some kind of something I didn't notice in the docs)
        packet.Write(1);
            
        registry.WriteToPacket(packet);

        packet.Write(tags.Count);
        foreach (Tag tag in tags)
        {
            tag.WriteToPacket(packet);
        }
            
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_tags"].Id);
        packet.WriteLength();

        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
        
        
        await new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}