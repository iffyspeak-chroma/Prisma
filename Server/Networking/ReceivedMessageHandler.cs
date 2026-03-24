using System.Diagnostics;
using API.Logging;
using API.Protocol.Packets;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Networking;

public class ReceivedMessageHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        // At this stage, if these are null, we are in big trouble.
        // This should be okay, but probably isn't.
        Debug.Assert(Server.Instance != null, "Server.Instance != null");
        Debug.Assert(Server.Instance.Configuration != null, "Server.Instance.Configuration != null");
        
        if (message is IByteBuffer data)
        {
            var raw = new byte[data.ReadableBytes];
            data.GetBytes(data.ReaderIndex, raw);
            
            using (Packet rMessage = new Packet(raw))
            {
                while (rMessage.UnreadLength() > 0)
                {
                    byte[] payloadData = rMessage.ReadBytes(rMessage.ReadVarInt());

                    try
                    {
                        using (Packet payload = new Packet(payloadData))
                        {
                            PacketManager.Instance.ReceivedPacket(context, payload);
                        }
                    }
                    catch (Exception exception)
                    {
                        LogTool.Exception(exception);
                    }
                }
            }
        }
    }
    
    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        LogTool.Exception(exception);
        LogTool.Error("Closing connection.");
        
        context.CloseAsync();
    }
}