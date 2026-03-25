using API.Core.Managers;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Clientbound;

public class ClientboundConfigurationFeatureFlagsPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_enabled_features"].Id);
            packet.WriteLength();
            
            await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);

            return;
        }
        
        List<string> featureIdentifiers = new List<string>()
        {
            "minecraft:vanilla"
        };
            
        packet.Write(featureIdentifiers.Count);
            
        foreach (string identifier in featureIdentifiers)
        {
            packet.Write(identifier);
        }
            
        packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:update_enabled_features"].Id);
        packet.WriteLength();
            
        await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
        
        await new ClientboundConfigurationKnownPacksPacket().Call(context, null);
    }
}