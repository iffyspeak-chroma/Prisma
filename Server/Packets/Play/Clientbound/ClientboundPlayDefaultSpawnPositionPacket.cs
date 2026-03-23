using API.DataTypes;
using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlayDefaultSpawnPositionPacket : ICallable
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_default_spawn_position"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        packet.Write("minecraft:overworld");
        
        packet.Write(new Position(0, 64, 0));
        
        packet.Write(0.0f);
        packet.Write(0.0f);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_default_spawn_position"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
        
        await new ClientboundPlayGameEventPacket().Call(context, null);
    }
}