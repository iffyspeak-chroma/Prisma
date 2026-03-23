using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayTickingStatePacket : ICallable
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:ticking_state"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        packet.Write(20f);
        packet.Write(false);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:ticking_state"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
    }
}