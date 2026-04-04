using System.Buffers.Binary;
using System.Text;
using API.Core;
using API.Logging;
using API.Protocol.Networking;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Handshake.Legacy;

public class LegacyClientboundStatusPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        List<byte> packetBytes = new List<byte>();
        
        packetBytes.Add(0xFF);

        List<string> responsePieces = new List<string>();
        
        responsePieces.Add("§1"); // Identifier
        responsePieces.Add("127"); // Protocol Version
        responsePieces.Add($"{Server.Instance?.VersionName}"); // Server version
        responsePieces.Add("Your client is outdated."); // MotD
        responsePieces.Add("0"); // Current player count
        responsePieces.Add("0"); // Max player count

        string msg = "";
        foreach (string piece in responsePieces)
        {
            msg += piece + "\0";
        }

        short length = (short) msg.Length;
        length = BinaryPrimitives.ReverseEndianness(length);
        packetBytes.AddRange(BitConverter.GetBytes(length));
        
        packetBytes.AddRange(Encoding.BigEndianUnicode.GetBytes(msg));
        
        var buffer = Unpooled.WrappedBuffer(packetBytes.ToArray());
        await context.Channel.WriteAndFlushAsync(buffer);

        await context.Channel.DisconnectAsync();
    }
}