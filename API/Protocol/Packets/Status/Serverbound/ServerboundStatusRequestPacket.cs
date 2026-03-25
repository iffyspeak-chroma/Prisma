using System.Text.Json;
using API.Protocol.Networking;
using API.Protocol.Packets.Status.Clientbound;
using API.Protocol.Status;
using API.Text;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Serverbound;

public class ServerboundStatusRequestPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        // TODO: We should only really be doing this if the server settings permit
        
        StatusResponseFormat status = new StatusResponseFormat();

        TextComponentBuilder builder = new TextComponentBuilder();
        builder.AddText("A Prisma Minecraft server", color: "green", bold: true);

        status.Description = builder;
        
        string response = JsonSerializer.Serialize(status);

        using (Packet p = new Packet())
        {
            p.Write(response);

            await new ClientboundStatusResponsePacket().Call(context, p);
        }
    }
}