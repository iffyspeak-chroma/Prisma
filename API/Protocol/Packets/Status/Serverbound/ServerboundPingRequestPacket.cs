using API.Protocol.Networking;
using API.Protocol.Packets.Status.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Serverbound;

public class ServerboundPingRequestPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            return;
        }
        
        long timestamp = packet.ReadLong();
        
        packet.Write(timestamp, asVarLong: false);

        await new ClientboundPongResponse().Call(context, packet);
    }
}