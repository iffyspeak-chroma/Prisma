using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayTickingStatePacket : ICallablePacket
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