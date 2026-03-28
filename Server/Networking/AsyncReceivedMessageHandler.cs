using System.Diagnostics;
using System.Net.Mime;
using API.Core.Managers;
using API.Game.Events;
using API.Logging;
using API.Protocol.Networking;
using API.Protocol.Packets;
using API.Protocol.Packets.Handshake.Legacy;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Server.Networking;

public class AsyncReceivedMessageHandler : ChannelHandlerAdapter
{
    private readonly EventDispatcher _events;

    public AsyncReceivedMessageHandler(EventDispatcher events)
    {
        _events = events;
    }
    
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        Debug.Assert(API.Core.Server.Instance != null, "Server.Instance != null");
        Debug.Assert(API.Core.Server.Instance.Configuration != null, "Server.Instance.Configuration != null");
        
        if (message is IByteBuffer data)
        {
            var raw = new byte[data.ReadableBytes];
            data.GetBytes(data.ReaderIndex, raw);
            

            if (raw[0] == 0xFE && raw[1] == 0x01 && raw[2] == 0xFA)
            {
                // Legacy status packet
                _ = new LegacyServerboundHandshakePacket().Call(context, new Packet(raw));
                return;
            }
            
            using (Packet rMessage = new Packet(raw))
            {
                while (rMessage.UnreadLength() > 0)
                {
                    byte[] payloadData = rMessage.ReadBytes(rMessage.ReadVarInt());

                    var payload = new Packet(payloadData);

                    context.Executor.Execute(async () =>
                    {
                        try
                        {
                            await PacketManager.Instance.ReceivedPacket(context, payload);
                        }
                        catch (Exception exception)
                        {
                            LogTool.Exception(exception);
                        }
                        finally
                        {
                            payload.Dispose();
                        }
                    });
                }
            }
        }
    }
    
    public override void ChannelReadComplete(IChannelHandlerContext context)
        => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        LogTool.Exception(exception);
        LogTool.Error("Closing connection.");
        
        PlayerManager.Instance.ConnectedClients[context.Channel].DisconnectReason = DisconnectReason.Exception;
        context.CloseAsync();
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        if (client.DisconnectReason == DisconnectReason.Generic)
            client.DisconnectReason = DisconnectReason.Disconnect;

        _events.Publish(new PlayerDisconnectEvent(client, client.DisconnectReason));
        PlayerManager.Instance.ConnectedClients.Remove(context.Channel);
        
        base.ChannelInactive(context);
    }
}