using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayInitializeBorderPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:initialize_border"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        // TODO: This should be done based on world settings
        
        // Border center XZ
        packet.Write(0d);
        packet.Write(0d);
        
        // Border old/new diameter
        packet.Write(60000000d);
        packet.Write(60000000d);
        
        // Contract/Expend time (ms)
        packet.Write(0L);
        
        // Portal teleport boundary
        packet.Write(29999984);
        
        // Warning blocks/time (s)
        packet.Write(0);
        packet.Write(0);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:initialize_border"].Id);
        packet.WriteLength();
        
        await client.SendPacket(packet);
        
        new ClientboundPlaySetTimePacket().Call(context, null);
    }
}