using API.Core;
using API.Core.Managers;
using API.DataPacks;
using API.Protocol.Mojang;
using API.Protocol.NBT;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;
using fNbt;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationRegistryDataPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:registry_data"].Id);
            packet.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
        }
        
        // I'm so sorry you put yourself through that, my friend.
        // I'm gonna fix this now... Manually entering in registries can't hurt you anymore
        
        foreach (Registry registry in RegistryManager.Instance.Registries)
        {
            registry.RegistryId.WriteToPacket(packet);
            
            packet.Write(registry.Entries.Count);

            foreach (RegistryEntry entry in registry.Entries)
            {
                entry.EntryId.WriteToPacket(packet);

                bool hasData = entry.Data != null;
                
                packet.Write(hasData);

                if (hasData && entry.Data != null)
                {
                    if (entry.Data.Name == null)
                    {
                        entry.Data.Name = "";
                    }
                    NbtFile file = new NbtFile(entry.Data);
                    packet.Write(NbtToolkit.StripUnnecessary(file.SaveToBuffer(NbtCompression.None)));
                }
                
                packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:registry_data"].Id);
                packet.WriteLength();
            
                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
            }
        }
        
        await new ClientboundConfigurationUpdateTagsPacket().Call(context, null);
    }
}