using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFinishPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:finish_configuration"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}