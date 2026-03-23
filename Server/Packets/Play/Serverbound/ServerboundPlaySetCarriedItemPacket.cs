using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlaySetCarriedItemPacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // TODO: Do something with this packet.
        return Task.CompletedTask;
    }
}