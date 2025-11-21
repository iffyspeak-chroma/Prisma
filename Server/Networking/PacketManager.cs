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
        if (message is not IByteBuffer data) return;
        
        byte[] buffer = new byte[data.ReadableBytes];
        data.GetBytes(data.ReaderIndex, buffer);

        using (Packet p = new Packet(buffer))
        {
            // Debugging information
            int length = p.ReadVarInt();
            p.SetReadPos(0);

            // At this stage, if these are null, we are in big trouble.
            // This should be okay, but probably isn't.
            Debug.Assert(Server.Instance != null, "Server.Instance != null");
            Debug.Assert(Server.Instance.Configuration != null, "Server.Instance.Configuration != null");
            
            LogTool.Debug(
                $"Packet is {length} bytes long.",
                Server.Instance.Configuration.DebugMode);
            
            LogTool.RawDebug($"Packet bytes: {Encoding.UTF8.GetString(buffer).Replace("-", "")}", 
                Server.Instance.Configuration.DebugMode, 
                ConsoleColor.Blue);
            
            // TODO: Actually handle the packet
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