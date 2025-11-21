using DotNetty.Transport.Channels;

namespace API.Networking;

public interface ICallable
{ 
    void Call(IChannelHandlerContext context, Packet? packet);
}