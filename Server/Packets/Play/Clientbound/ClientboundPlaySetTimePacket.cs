using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlaySetTimePacket : ICallable
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_time"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        // TODO: These should in theory come from the world the player is in
        
        // World tick age
        packet.Write(0L);
        
        // Region time
        /*
         * Time is based on ticks, where 20 ticks happen every second. There are 24000 ticks in a day, making Minecraft days exactly 20 minutes long.
         * The time of day is based on the timestamp modulo 24000. 0 is sunrise, 6000 is noon, 12000 is sunset, and 18000 is midnight.
         * The default SMP server increments the time by 20 every second.
         *
         * https://minecraft.wiki/w/Java_Edition_protocol/Packets#Update_Time
         */
        packet.Write(0L);
        
        // Advance daylight cycle
        packet.Write(false);
        
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_time"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
        
        new ClientboundPlayDefaultSpawnPositionPacket().Call(context, packet);
    }
}