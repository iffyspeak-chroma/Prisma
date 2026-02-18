using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationPluginMessagePacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            p.Write("minecraft:brand"); // Identifier
            p.Write("Prisma"); // Data
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:custom_payload"].Id);
            p.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}