using System.Diagnostics;
using System.Text;
using API.Logging;
using API.Networking;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Server.Networking;

public class PacketManager : ChannelHandlerAdapter
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
            
            using (Packet p = new Packet(raw))
            {
                // Debugging information
                int length = p.ReadVarInt();
                p.SetReadPos(0);
            
                LogTool.Debug(
                    $"Packet is {length} bytes long.",
                    Server.Instance.Configuration.DebugMode);
            
                LogTool.RawDebug($"Packet bytes: {BitConverter.ToString(raw).Replace("-", "")}", 
                    Server.Instance.Configuration.DebugMode, 
                    ConsoleColor.Blue);
            
                // TODO: Actually handle the packet
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