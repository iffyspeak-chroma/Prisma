using API.Protocol.Networking;
using API.Protocol.Packets;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayPlayerAbilitiesPacket : ICallablePacket
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return Task.CompletedTask;
        
        // TODO: Do something with this packet.
        
        return Task.CompletedTask;
    }
}