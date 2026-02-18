using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayLoginPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        packet = new Packet();
        
        
    }
}