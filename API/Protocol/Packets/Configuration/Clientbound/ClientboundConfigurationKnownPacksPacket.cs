using API.Core;
using API.Core.Managers;
using API.DataPacks;
using API.Protocol.Mojang;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationKnownPacksPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:select_known_packs"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        List<DataPackIdentity> datapacks = new List<DataPackIdentity>();
            
        DataPackIdentity corePack = new DataPackIdentity(
            Identifier.Parse("minecraft:core"),
            $"{Server.Instance?.VersionName}");
            
        datapacks.Add(corePack);

        packet.Write(datapacks.Count);
        foreach (DataPackIdentity pack in datapacks)
        {
            packet.Write(pack.Namespace);
            packet.Write(pack.Id);
            packet.Write(pack.Version);
        }
            
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:select_known_packs"].Id);
        packet.WriteLength();

        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
    }
}