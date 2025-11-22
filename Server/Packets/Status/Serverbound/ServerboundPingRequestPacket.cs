using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Status.Clientbound;

namespace Server.Packets.Status.Serverbound;

public class ServerboundPingRequestPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        LogTool.Debug($"{context.Channel.RemoteAddress} sent a ping request", Server.Instance.Configuration.DebugMode);
        
        long timestamp = packet.ReadLong();
        
        using (Packet p = new Packet())
        {
            p.Write(timestamp, asVarLong: false);

            new ClientboundPongResponse().Call(context, p);
        }
    }
}