using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Status.Clientbound;

namespace Server.Packets.Status.Serverbound;

public class ServerboundPingRequestPacket : ICallable
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        long timestamp = packet.ReadLong();
        
        using (Packet p = new Packet())
        {
            p.Write(timestamp, asVarLong: false);

            await new ClientboundPongResponse().Call(context, p);
        }
    }
}