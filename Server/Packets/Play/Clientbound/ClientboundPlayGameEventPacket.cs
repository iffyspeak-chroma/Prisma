using API.DataTypes;
using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayGameEventPacket : ICallable
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:game_event"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        packet.Write(GameEvents.WaitForChunks);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:game_event"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
    }
}