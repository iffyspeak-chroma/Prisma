using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Status.Clientbound;

namespace Server.Packets.Status.Serverbound;

public class ServerboundPingRequestPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            return;
        }
        
        long timestamp = packet.ReadLong();
        
        using (Packet p = new Packet())
        {
            p.Write(timestamp);

            new ClientboundPongResponse().Call(context, p);
        }
    }
}