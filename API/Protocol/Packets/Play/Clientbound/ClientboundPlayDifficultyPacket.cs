using API.Core.Managers;
using API.Game;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayDifficultyPacket : ICallablePacket
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
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:change_difficulty"].Id);
            packet.WriteLength();
            
            await client.SendPacket(packet);

            return;
        }
        
        // TODO: Proper implementation
        packet.Write(GameDifficulty.Peaceful);
        packet.Write(true);

        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:change_difficulty"].Id);
        packet.WriteLength();
            
        await client.SendPacket(packet);
            
        await new ClientboundPlayPlayerAbilitiesPacket().Call(context, null);
    }
}