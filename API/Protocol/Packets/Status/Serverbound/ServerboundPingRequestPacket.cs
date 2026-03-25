using API.Protocol.Networking;
using API.Protocol.Packets.Status.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Serverbound;

public class ServerboundPingRequestPacket : ICallablePacket
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