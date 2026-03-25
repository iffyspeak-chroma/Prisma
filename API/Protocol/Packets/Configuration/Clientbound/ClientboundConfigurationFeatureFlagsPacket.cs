using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFeatureFlagsPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        using (Packet p = new Packet())
        {
            List<string> featureIdentifiers = new List<string>()
            {
                "minecraft:vanilla"
            };
            
            p.Write(featureIdentifiers.Count);
            
            foreach (string identifier in featureIdentifiers)
            {
                p.Write(identifier);
            }
            
            p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_enabled_features"].Id);
            p.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(p);
        }
        
        await new ClientboundConfigurationKnownPacksPacket().Call(context, null);
    }
}