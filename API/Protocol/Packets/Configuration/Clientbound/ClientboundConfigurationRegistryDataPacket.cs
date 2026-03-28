using API.Core.Managers;
using API.Protocol.Mojang;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationRegistryDataPacket : ICallablePacket
{
    private List<Packet> _registrySync = new List<Packet>();

    private Packet GenerateRegistryDataPacket(Identifier identifier, List<Identifier> identities)
    {
        Packet syncPacket = new Packet();
        
        syncPacket.Write(identifier.ToString());
        
        syncPacket.Write(identities.Count);
        foreach (Identifier identity in identities)
        {
            syncPacket.Write(identity.ToString());
            
            // Don't send the optional NBT (because we're mean and evil)
            syncPacket.Write(false);
        }

        return syncPacket;
    }
    
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // What is about to happen here is nasty and gross to me but
        // gets me to the next phase with minimal brain effort.
        
        #region Dimension Types
        
        List<Identifier> dimensionTypeIdentities = new List<Identifier>()
        {
            Identifier.Parse("minecraft:the_nether"), 
            Identifier.Parse("minecraft:overworld"),
            Identifier.Parse("minecraft:the_end")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:dimension_type"), dimensionTypeIdentities));

        #endregion
        
        #region Damage Types
        
        List<Identifier> damageTypeIdentities = new List<Identifier>()
        {
            Identifier.Parse("minecraft:cactus"),
            Identifier.Parse("minecraft:campfire"),
            Identifier.Parse("minecraft:cramming"),
            Identifier.Parse("minecraft:dragon_breath"),
            Identifier.Parse("minecraft:drown"),
            Identifier.Parse("minecraft:dry_out"),
            Identifier.Parse("minecraft:ender_pearl"),
            Identifier.Parse("minecraft:fall"),
            Identifier.Parse("minecraft:fly_into_wall"),
            Identifier.Parse("minecraft:freeze"),
            Identifier.Parse("minecraft:generic"),
            Identifier.Parse("minecraft:generic_kill"),
            Identifier.Parse("minecraft:hot_floor"),
            Identifier.Parse("minecraft:in_fire"),
            Identifier.Parse("minecraft:in_wall"),
            Identifier.Parse("minecraft:lava"),
            Identifier.Parse("minecraft:lightning_bolt"),
            Identifier.Parse("minecraft:magic"),
            Identifier.Parse("minecraft:on_fire"),
            Identifier.Parse("minecraft:out_of_world"),
            Identifier.Parse("minecraft:outside_border"),
            Identifier.Parse("minecraft:stalagmite"),
            Identifier.Parse("minecraft:spear"),
            Identifier.Parse("minecraft:starve"),
            Identifier.Parse("minecraft:sweet_berry_bush"),
            Identifier.Parse("minecraft:wither")
        };

        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:damage_type"), damageTypeIdentities));
        
        #endregion

        #region Painting Variants

        List<Identifier> paintingVariantIdentities = new List<Identifier>()
        {
            Identifier.Parse("minecraft:kebab"),
            Identifier.Parse("minecraft:aztec"),
            Identifier.Parse("minecraft:alban"),
            Identifier.Parse("minecraft:aztec2"),
            Identifier.Parse("minecraft:bomb"),
            Identifier.Parse("minecraft:plant"),
            Identifier.Parse("minecraft:wasteland"),
            Identifier.Parse("minecraft:meditative"),
            Identifier.Parse("minecraft:wanderer"),
            Identifier.Parse("minecraft:graham"),
            Identifier.Parse("minecraft:prairie_ride"),
            Identifier.Parse("minecraft:pool"),
            Identifier.Parse("minecraft:courbet"),
            Identifier.Parse("minecraft:sunset"),
            Identifier.Parse("minecraft:sea"),
            Identifier.Parse("minecraft:creebet"),
            Identifier.Parse("minecraft:match"),
            Identifier.Parse("minecraft:bust"),
            Identifier.Parse("minecraft:stage"),
            Identifier.Parse("minecraft:void"),
            Identifier.Parse("minecraft:skull_and_roses"),
            Identifier.Parse("minecraft:wither"),
            Identifier.Parse("minecraft:baroque"),
            Identifier.Parse("minecraft:humble"),
            Identifier.Parse("minecraft:bouquet"),
            Identifier.Parse("minecraft:cavebird"),
            Identifier.Parse("minecraft:cotan"),
            Identifier.Parse("minecraft:endboss"),
            Identifier.Parse("minecraft:fern"),
            Identifier.Parse("minecraft:owlemons"),
            Identifier.Parse("minecraft:sunflowers"),
            Identifier.Parse("minecraft:tides"),
            Identifier.Parse("minecraft:dennis"),
            Identifier.Parse("minecraft:backyard"),
            Identifier.Parse("minecraft:pond"),
            Identifier.Parse("minecraft:fighters"),
            Identifier.Parse("minecraft:changing"),
            Identifier.Parse("minecraft:finding"),
            Identifier.Parse("minecraft:lowmist"),
            Identifier.Parse("minecraft:passage"),
            Identifier.Parse("minecraft:skeleton"),
            Identifier.Parse("minecraft:donkey_kong"),
            Identifier.Parse("minecraft:pointer"),
            Identifier.Parse("minecraft:pigscene"),
            Identifier.Parse("minecraft:burning_skull"),
            Identifier.Parse("minecraft:orb"),
            Identifier.Parse("minecraft:unpacked")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:painting_variant"), paintingVariantIdentities));

        #endregion
        
        #region World Generation / Biome

        List<Identifier> biomeIdentityList = new List<Identifier>()
        {
            // TODO: Biomes list brrr (https://minecraft.wiki/w/Biome)
            Identifier.Parse("minecraft:plains")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:worldgen/biome"), biomeIdentityList));
        
        #endregion

        #region Animal Variants

        #region Cat Variants

        List<Identifier> catIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:white"),
            Identifier.Parse("minecraft:black"),
            Identifier.Parse("minecraft:red"),
            Identifier.Parse("minecraft:siamese"),
            Identifier.Parse("minecraft:british_shorthair"),
            Identifier.Parse("minecraft:calico"),
            Identifier.Parse("minecraft:persian"),
            Identifier.Parse("minecraft:ragdoll"),
            Identifier.Parse("minecraft:tabby"),
            Identifier.Parse("minecraft:all_black"),
            Identifier.Parse("minecraft:jellie")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:cat_variant"), catIdentityList));

        List<Identifier> catSoundIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:classic"),
            Identifier.Parse("minecraft:royal")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:cat_sound_variant"), catSoundIdentityList));

        #endregion
        
        #region Chicken Variants

        List<Identifier> chickenIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:cold"),
            Identifier.Parse("minecraft:temperate"),
            Identifier.Parse("minecraft:warm")
        };

        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:chicken_variant"), chickenIdentityList));
        
        List<Identifier> chickenSoundIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:classic"),
            Identifier.Parse("minecraft:picky")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:chicken_sound_variant"), chickenSoundIdentityList));
        
        #endregion

        #region Cow Variants

        List<Identifier> cowIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:cold"),
            Identifier.Parse("minecraft:temperate"),
            Identifier.Parse("minecraft:warm")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:cow_variant"), cowIdentityList));
        
        List<Identifier> cowSoundIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:classic"),
            Identifier.Parse("minecraft:moody")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:cow_sound_variant"), cowSoundIdentityList));

        #endregion

        #region Frog Variants

        List<Identifier> frogIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:cold"),
            Identifier.Parse("minecraft:temperate"),
            Identifier.Parse("minecraft:warm")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:frog_variant"), frogIdentityList));

        #endregion

        #region Pig Variants

        List<Identifier> pigIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:cold"),
            Identifier.Parse("minecraft:temperate"),
            Identifier.Parse("minecraft:warm")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:pig_variant"), pigIdentityList));
        
        List<Identifier> pigSoundIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:classic"),
            Identifier.Parse("minecraft:mini"),
            Identifier.Parse("minecraft:big")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:pig_sound_variant"), pigSoundIdentityList));

        #endregion

        #region Wolf Variants

        List<Identifier> wolfIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:pale"),
            Identifier.Parse("minecraft:ashen"),
            Identifier.Parse("minecraft:black"),
            Identifier.Parse("minecraft:chestnut"),
            Identifier.Parse("minecraft:rusty"),
            Identifier.Parse("minecraft:snowy"),
            Identifier.Parse("minecraft:spotted"),
            Identifier.Parse("minecraft:striped"),
            Identifier.Parse("minecraft:woods")
        };

        List<Identifier> wolfSoundIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:classic"),
            Identifier.Parse("minecraft:angry"),
            Identifier.Parse("minecraft:big"),
            Identifier.Parse("minecraft:cute"),
            Identifier.Parse("minecraft:grumpy"),
            Identifier.Parse("minecraft:puglin"),
            Identifier.Parse("minecraft:sad")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:wolf_variant"), wolfIdentityList));
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:wolf_sound_variant"), wolfSoundIdentityList));

        #endregion

        #region Zombie Nautilus Variants

        List<Identifier> zombieNautilusIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:temperate"),
            Identifier.Parse("minecraft:warm")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:zombie_nautilus_variant"), zombieNautilusIdentityList));

        #endregion

        #endregion

        #region Timeline

        List<Identifier> timelineIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:day"),
            Identifier.Parse("minecraft:early_game"),
            Identifier.Parse("minecraft:moon"),
            Identifier.Parse("minecraft:villager_schedule")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:timeline"), timelineIdentityList));

        #endregion

        #region World Clock

        List<Identifier> clockIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:the_end"),
            Identifier.Parse("minecraft:overworld")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:world_clock"), clockIdentityList));

        #endregion

        #region Trim Materials

        List<Identifier> trimMaterialIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:amethyst"),
            Identifier.Parse("minecraft:copper"),
            Identifier.Parse("minecraft:diamond"),
            Identifier.Parse("minecraft:emerald"),
            Identifier.Parse("minecraft:gold"),
            Identifier.Parse("minecraft:iron"),
            Identifier.Parse("minecraft:lapis"),
            Identifier.Parse("minecraft:netherite"),
            Identifier.Parse("minecraft:quartz"),
            Identifier.Parse("minecraft:redstone"),
            Identifier.Parse("minecraft:resin"),
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:trim_material"), trimMaterialIdentityList));

        #endregion

        #region Jukebox Discs

        List<Identifier> discIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:5"),
            Identifier.Parse("minecraft:11"),
            Identifier.Parse("minecraft:13"),
            Identifier.Parse("minecraft:blocks"),
            Identifier.Parse("minecraft:cat"),
            Identifier.Parse("minecraft:chirp"),
            Identifier.Parse("minecraft:creator"),
            Identifier.Parse("minecraft:creator_music_box"),
            Identifier.Parse("minecraft:far"),
            Identifier.Parse("minecraft:lava_chicken"),
            Identifier.Parse("minecraft:mall"),
            Identifier.Parse("minecraft:mellohi"),
            Identifier.Parse("minecraft:otherside"),
            Identifier.Parse("minecraft:pigstep"),
            Identifier.Parse("minecraft:precipice"),
            Identifier.Parse("minecraft:relic"),
            Identifier.Parse("minecraft:stal"),
            Identifier.Parse("minecraft:strad"),
            Identifier.Parse("minecraft:tears"),
            Identifier.Parse("minecraft:wait"),
            Identifier.Parse("minecraft:ward")
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:jukebox_song"), discIdentityList));

        #endregion

        #region Instruments

        List<Identifier> instrumentIdentityList = new List<Identifier>()
        {
            Identifier.Parse("minecraft:admire_goat_horn"),
            Identifier.Parse("minecraft:call_goat_horn"),
            Identifier.Parse("minecraft:dream_goat_horn"),
            Identifier.Parse("minecraft:feel_goat_horn"),
            Identifier.Parse("minecraft:ponder_goat_horn"),
            Identifier.Parse("minecraft:seek_goat_horn"),
            Identifier.Parse("minecraft:sing_goat_horn"),
            Identifier.Parse("minecraft:yearn_goat_horn"),
        };
        
        _registrySync.Add(GenerateRegistryDataPacket(Identifier.Parse("minecraft:instrument"), instrumentIdentityList));

        #endregion

        foreach (Packet p in _registrySync)
        {
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:registry_data"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
        
        await new ClientboundConfigurationUpdateTagsPacket().Call(context, null);
        //new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}