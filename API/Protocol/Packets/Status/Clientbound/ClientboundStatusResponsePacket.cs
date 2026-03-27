using System.Text.Json;
using API.Core.Managers;
using API.Logging;
using API.Protocol.Networking;
using API.Protocol.Status;
using API.Text;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Status.Clientbound;

public class ClientboundStatusResponsePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:status_response"].Id);
            packet.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

            return;
        }
        
        // TODO: We should only really be doing this if the server settings permit
        // TODO: Should also properly write the values in here.
        
        StatusResponseFormat status = new StatusResponseFormat();

        TextComponentBuilder builder = new TextComponentBuilder();
        builder.AddText("A Prisma Minecraft server", color: "green", bold: true);

        status.Description = builder;
        
        string response = JsonSerializer.Serialize(status);

        packet.Write(response);

        packet.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:status_response"].Id);
        packet.WriteLength();
            
        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
    }
}