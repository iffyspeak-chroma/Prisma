using DotNetty.Transport.Channels;

namespace API.Networking;

public interface ICallable
{ 
    Task Call(IChannelHandlerContext context, Packet? packet);
}