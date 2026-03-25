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
        
        using (Packet p = new Packet())
        {
            // TODO: Proper implementation
            p.Write(GameDifficulty.Peaceful);
            p.Write(true);

            p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:change_difficulty"].Id);
            p.WriteLength();
            
            await client.SendPacket(p);
            
            await new ClientboundPlayPlayerAbilitiesPacket().Call(context, null);
        }
    }
}