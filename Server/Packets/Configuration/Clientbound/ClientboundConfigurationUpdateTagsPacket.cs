using API.Protocol.Mojang;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationUpdateTagsPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
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

        using (Packet p = new Packet())
        {
            // Only writing to 1 registry? (Some kind of something I didn't notice in the docs)
            p.Write(1);
            
            registry.WriteToPacket(p);

            p.Write(tags.Count);
            foreach (Tag tag in tags)
            {
                tag.WriteToPacket(p);
            }
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_tags"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
        
        await new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}