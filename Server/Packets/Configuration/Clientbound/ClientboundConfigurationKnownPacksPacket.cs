using API.DataPacks;
using API.Protocol.Mojang;
using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationKnownPacksPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            List<DataPackIdentity> datapacks = new List<DataPackIdentity>();
            
            DataPackIdentity corePack = new DataPackIdentity(
                Identifier.Parse("minecraft:core"),
                "1.21.11_unobfuscated");
            
            datapacks.Add(corePack);

            p.Write(datapacks.Count);
            foreach (DataPackIdentity pack in datapacks)
            {
                p.Write(pack.Namespace);
                p.Write(pack.Id);
                p.Write(pack.Version);
            }
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:select_known_packs"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}