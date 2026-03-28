using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Handshake.Legacy;

public class LegacyServerboundHandshakePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // I mean, I *could* like do some kinda implementation but why?
        // I'm eventually gonna tell them no at the door anyway.

        await new LegacyClientboundStatusPacket().Call(context, null);
    }
}