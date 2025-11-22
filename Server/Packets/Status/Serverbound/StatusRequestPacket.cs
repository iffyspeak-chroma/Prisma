using System.Text.Json;
using API.DataTypes;
using API.Logging;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;

namespace Server.Packets.Status.Serverbound;

public class StatusRequestPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        LogTool.Debug($"{context.Channel.RemoteAddress} sent a status request", Server.Instance.Configuration.DebugMode);

        // TODO: We should only really be doing this if the server settings permit
        
        StatusResponseFormat status = new StatusResponseFormat();

        TextComponentBuilder builder = new TextComponentBuilder();
        builder.AddText("A Prisma Minecraft server", color: "green", bold: true);

        status.Description = builder;
        
        string response = JsonSerializer.Serialize(status);

        using (Packet p = new Packet())
        {
            p.Write(response);
            
            
        }
    }
}