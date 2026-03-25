using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Serverbound;

public class ServerboundPlaySetCarriedItemPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // TODO: Do something with this packet.
        return Task.CompletedTask;
    }
}