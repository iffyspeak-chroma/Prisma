using API.Protocol.Networking;
using API.Protocol.Packets.GamestateIndependent.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.GamestateIndependent.Serverbound;

public class ServerboundPingRequestPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            throw new InvalidOperationException("Packet should not be null in this context.");
        }
        
        long timestamp = packet.ReadLong();
        packet = new Packet();
        
        packet.Write(timestamp, asVarLong: false);

        await new ClientboundPongResponsePacket().Call(context, packet);
    }
}