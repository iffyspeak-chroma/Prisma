using API.Core.Managers;
using API.Game.Events;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayGameEventPacket : ICallablePacket
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:game_event"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);
            return;
        }
        
        // Event type and event options.
        packet.Write(GameEvents.WaitForChunks);
        packet.Write(GameEventOptions.Generic.Empty);
        
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:game_event"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
        
        await new ClientboundPlayTickingStatePacket().Call(context, null);
        await new ClientboundPlaySetCenterChunkPacket().Call(context, null);
    }
}