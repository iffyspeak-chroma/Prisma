using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlaySetCarriedItemPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        // TODO: Do something with this packet.
    }
}