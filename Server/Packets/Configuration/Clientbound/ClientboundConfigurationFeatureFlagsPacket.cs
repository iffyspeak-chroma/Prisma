using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFeatureFlagsPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
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
        
        new ClientboundConfigurationKnownPacksPacket().Call(context, null);
    }
}