using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Login.Clientbound;

public class ClientboundLoginSuccessPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet(packet.ToArray()))
        {
            p.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_finished"].Id);
            p.WriteLength();

            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
    }
}