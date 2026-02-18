using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationRegistryDataPacket : ICallable
{
    private List<Packet> _registrySync = new List<Packet>();

    private Packet GenerateRegistryDataPacket(string identifier, List<string> identities)
    {
        Packet syncPacket = new Packet();
        
        syncPacket.Write(identifier);
        syncPacket.Write(identities.Count);

        foreach (string identity in identities)
        {
            syncPacket.Write(identity);
            syncPacket.Write(false);
        }

        return syncPacket;
    }
    
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        // What is about to happen here is nasty and gross to me but
        // gets me to the next phase with minimal brain effort.
        
        #region Damage Types
        
        List<string> damageTypeIdentities = new List<string>()
        {
            "minecraft:cactus",
            "minecraft:campfire",
            "minecraft:cramming",
            "minecraft:dragon_breath",
            "minecraft:drown",
            "minecraft:dry_out",
            "minecraft:ender_pearl",
            "minecraft:fall",
            "minecraft:fly_into_wall",
            "minecraft:freeze",
            "minecraft:generic",
            "minecraft:generic_kill",
            "minecraft:hot_floor",
            "minecraft:in_fire",
            "minecraft:in_wall",
            "minecraft:lava",
            "minecraft:lightning_bolt",
            "minecraft:magic",
            "minecraft:on_fire",
            "minecraft:out_of_world",
            "minecraft:outside_border",
            "minecraft:stalagmite",
            "minecraft:starve",
            "minecraft:sweet_berry_bush",
            "minecraft:wither"
        };

        _registrySync.Add(GenerateRegistryDataPacket("minecraft:damage_type", damageTypeIdentities));
        
        #endregion

        #region Dimension Types
        
        List<string> dimensionTypeIdentities = new List<string>()
        {
            "minecraft:overworld",
            "minecraft:the_nether",
            "minecraft:the_end"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:dimension_type", dimensionTypeIdentities));

        #endregion

        #region Painting Variants

        List<string> paintingVariantIdentities = new List<string>()
        {
            "minecraft:kebab",
            "minecraft:aztec",
            "minecraft:alban",
            "minecraft:aztec2",
            "minecraft:bomb",
            "minecraft:plant",
            "minecraft:wasteland",
            "minecraft:meditative",
            "minecraft:wanderer",
            "minecraft:graham",
            "minecraft:prairie_ride",
            "minecraft:pool",
            "minecraft:courbet",
            "minecraft:sunset",
            "minecraft:sea",
            "minecraft:creebet",
            "minecraft:match",
            "minecraft:bust",
            "minecraft:stage",
            "minecraft:void",
            "minecraft:skull_and_roses",
            "minecraft:wither",
            "minecraft:baroque",
            "minecraft:humble",
            "minecraft:bouquet",
            "minecraft:cavebird",
            "minecraft:cotan",
            "minecraft:endboss",
            "minecraft:fern",
            "minecraft:owlmelons",
            "minecraft:sunflowers",
            "minecraft:tides",
            "minecraft:dennis",
            "minecraft:backyard",
            "minecraft:pond",
            "minecraft:fighters",
            "minecraft:changing",
            "minecraft:finding",
            "minecraft:lowmist",
            "minecraft:passage",
            "minecraft:skeleton",
            "minecraft:donkey_kong",
            "minecraft:pointer",
            "minecraft:pigscene",
            "minecraft:burning_skull",
            "minecraft:orb",
            "minecraft:unpacked"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:painting_variant", paintingVariantIdentities));

        #endregion
        
        #region World Generation / Biome

        List<string> biomeIdentityList = new List<string>()
        {
            // TODO: Biomes list brrr (https://minecraft.wiki/w/Biome)
            "minecraft:plains"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:worldgen/biome", biomeIdentityList));
        
        #endregion

        #region Animal Variants

        #region Cat Variants

        List<string> catIdentityList = new List<string>()
        {
            "minecraft:white",
            "minecraft:black",
            "minecraft:red",
            "minecraft:siamese",
            "minecraft:british_shorthair",
            "minecraft:calico",
            "minecraft:persian",
            "minecraft:ragdoll",
            "minecraft:tabby",
            "minecraft:all_black",
            "minecraft:jellie"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:cat_variant", catIdentityList));

        #endregion
        
        #region Chicken Variants

        List<string> chickenIdentityList = new List<string>()
        {
            "minecraft:cold",
            "minecraft:temperate",
            "minecraft:warm"
        };

        _registrySync.Add(GenerateRegistryDataPacket("minecraft:chicken_variant", chickenIdentityList));
        
        #endregion

        #region Cow Variants

        List<string> cowIdentityList = new List<string>()
        {
            "minecraft:cold",
            "minecraft:temperate",
            "minecraft:warm"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:cow_variant", cowIdentityList));

        #endregion

        #region Frog Variants

        List<string> frogIdentityList = new List<string>()
        {
            "minecraft:cold",
            "minecraft:temperate",
            "minecraft:warm"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:frog_variant", frogIdentityList));

        #endregion

        #region Pig Variants

        List<string> pigIdentityList = new List<string>()
        {
            "minecraft:cold",
            "minecraft:temperate",
            "minecraft:warm"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:pig_variant", pigIdentityList));

        #endregion

        #region Wolf Variants

        List<string> wolfIdentityList = new List<string>()
        {
            "minecraft:pale",
            "minecraft:ashen",
            "minecraft:black",
            "minecraft:chestnut",
            "minecraft:rusty",
            "minecraft:snowy",
            "minecraft:spotted",
            "minecraft:striped",
            "minecraft:woods"
        };

        List<string> wolfSoundIdentityList = new List<string>()
        {
            "minecraft:classic",
            "minecraft:angry",
            "minecraft:big",
            "minecraft:cute",
            "minecraft:grumpy",
            "minecraft:puglin",
            "minecraft:sad"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:wolf_variant", wolfIdentityList));
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:wolf_sound_variant", wolfSoundIdentityList));

        #endregion

        #region Zombie Nautilus Variants

        List<string> zombieNautilusIdentityList = new List<string>()
        {
            "minecraft:temperate",
            "minecraft:warm"
        };
        
        _registrySync.Add(GenerateRegistryDataPacket("minecraft:zombie_nautilus_variant", zombieNautilusIdentityList));

        #endregion

        #endregion

        foreach (Packet p in _registrySync)
        {
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:registry_data"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
        
        new ClientboundConfigurationFinishPacket().Call(context, null);
    }
}