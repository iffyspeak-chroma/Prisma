using System.Diagnostics;
using System.Text;
using API.Logging;
using API.Networking;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Server.Networking;

public class PacketHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        // At this stage, if these are null, we are in big trouble.
        // This should be okay, but probably isn't.
        Debug.Assert(Server.Instance != null, "Server.Instance != null");
        Debug.Assert(Server.Instance.Configuration != null, "Server.Instance.Configuration != null");
        
        LogTool.Debug($"Got a message from {context.Channel.RemoteAddress}", Server.Instance.Configuration.DebugMode);
        
        if (message is IByteBuffer data)
        {
            var raw = new byte[data.ReadableBytes];
            data.GetBytes(data.ReaderIndex, raw);
            
            LogTool.Debug($"Message length is {raw.Length} bytes\nMessage bytes: {BitConverter.ToString(raw).Replace("-", "")}", Server.Instance.Configuration.DebugMode);
            
            using (Packet rMessage = new Packet(raw))
            {
                while (rMessage.UnreadLength() > 0)
                {
                    byte[] payloadData = rMessage.ReadBytes(rMessage.ReadVarInt());
                    LogTool.Debug($"Payload length is {payloadData.Length} bytes\nMessage bytes: {BitConverter.ToString(payloadData).Replace("-", "")}", Server.Instance.Configuration.DebugMode);

                    try
                    {
                        using (Packet payload = new Packet(payloadData))
                        {
                            // TODO: Handle packet here.
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