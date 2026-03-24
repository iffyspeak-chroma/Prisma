using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;

namespace Server.Packets.Play.Serverbound;

public class ServerboundPlayPlayerAbilitiesPacket : ICallable
{
    public Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
            return Task.CompletedTask;
        
        // TODO: Do something with this packet.
        
        return Task.CompletedTask;
    }
}