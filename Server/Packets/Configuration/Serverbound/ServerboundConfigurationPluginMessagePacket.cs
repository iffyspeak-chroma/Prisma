using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationPluginMessagePacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        throw new NotImplementedException();
    }
}