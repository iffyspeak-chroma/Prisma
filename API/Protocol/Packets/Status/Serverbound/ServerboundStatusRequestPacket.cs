using API.Protocol.Networking;
using API.Protocol.Packets.Status.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Serverbound;

public class ServerboundStatusRequestPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            await new ClientboundStatusResponsePacket().Call(context, null);
            return;
        }

        await new ClientboundStatusResponsePacket().Call(context, null);
    }
}