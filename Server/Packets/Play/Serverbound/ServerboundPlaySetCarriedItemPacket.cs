using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlaySetCarriedItemPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // TODO: Do something with this packet.
        return Task.CompletedTask;
    }
}