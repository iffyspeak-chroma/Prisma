using API.DataTypes.DataPacks;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationKnownPacksPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            List<DataPackIdentity> datapacks = new List<DataPackIdentity>();
            
            DataPackIdentity corePack = new DataPackIdentity();
            
            corePack.Namespace = "minecraft";
            corePack.ID = "core";
            corePack.Version = "1.21.11";
            
            datapacks.Add(corePack);

            p.Write(datapacks.Count);
            foreach (DataPackIdentity pack in datapacks)
            {
                p.Write(pack.Namespace);
                p.Write(pack.ID);
                p.Write(pack.Version);
            }
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:select_known_packs"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}