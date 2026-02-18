using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFinishPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        throw new NotImplementedException();
    }
}