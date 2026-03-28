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

        Dictionary<Identifier, List<Tag>> registries = new Dictionary<Identifier, List<Tag>>();

        #region Timeline Tags

        Identifier timeline = new Identifier("minecraft", "timeline");
        List<Tag> timeline_tags = new List<Tag>()
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
        
        registries.Add(timeline, timeline_tags);

        #endregion

        #region Damage Tags

        Identifier damage_type = new Identifier("minecraft", "damage_type");
        List<Tag> damage_type_tags = new List<Tag>()
        {
            new Tag(Identifier.Parse("minecraft:always_hurts_ender_dragons"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:always_kills_armor_stands"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:always_most_significant_fall"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:always_triggers_silverfish"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:avoids_guardian_thorns"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:burns_armor_stands"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_armor"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_cooldown"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_effects"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_enchantments"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_invulnerability"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_resistance"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:bypasses_shield"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:damages_helmet"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:ignites_armor_stands"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_drowning"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_explosion"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_fall"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_fire"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_freezing"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_lightning"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:is_projectile"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:no_anger"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:no_impact"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:no_knockback"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:witch_resistant_to"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:wither_resistant_to"), new List<int>()),
        };
        
        registries.Add(damage_type, damage_type_tags);

        #endregion

        #region Banner Pattern Item Tags

        Identifier pattern_item = new Identifier("minecraft", "banner_pattern");
        List<Tag> pattern_tags = new List<Tag>()
        {
            new Tag(Identifier.Parse("minecraft:pattern_item/creeper"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/flower"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/globe"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/mojang"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/piglin"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/skull"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/flow"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/guster"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/field_masoned"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/bordure_indented"), new List<int>()),
            new Tag(Identifier.Parse("minecraft:pattern_item/no_item_required"), new List<int>()),
        };
        
        registries.Add(pattern_item, pattern_tags);

        #endregion
        
        packet.Write(registries.Count);

        foreach (var registry in registries)
        {
            Identifier id = registry.Key;
            List<Tag> tags = registry.Value;
            
            id.WriteToPacket(packet);
            
            packet.Write(tags.Count);
            foreach (Tag tag in tags)
            {
                tag.WriteToPacket(packet);
            }
        }
        
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_tags"].Id);
        packet.WriteLength();

        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
        
        
        await new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}