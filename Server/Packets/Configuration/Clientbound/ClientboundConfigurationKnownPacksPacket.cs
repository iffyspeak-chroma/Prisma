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
            p.Write("minecraft");
            p.Write("core");
            p.Write("1.21.11");
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:select_known_packs"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}