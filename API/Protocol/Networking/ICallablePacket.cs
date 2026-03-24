using API.Protocol.Packets;
using DotNetty.Transport.Channels;

namespace API.Protocol.Networking;

public interface ICallablePacket
{ 
    Task Call(IChannelHandlerContext context, Packet? packet);
}