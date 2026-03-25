using API.Core.Managers;
using API.DataPacks;
using API.Logging;
using API.Player;
using API.Protocol.Mojang;
using API.Protocol.Networking;
using API.Protocol.Packets.Configuration.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Configuration.Serverbound;

public class ServerboundConfigurationKnownPacksPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        List<DataPackIdentity> packs = new List<DataPackIdentity>();
        
        int packCount = packet.ReadVarInt();
        for (int i = packCount; i > 0; i--)
        {
            Identifier identifier = Identifier.Parse($"{packet.ReadString()}:{packet.ReadString()}");
            DataPackIdentity next = new DataPackIdentity(identifier, packet.ReadString());
            
            packs.Add(next);
        }
        
        LogTool.Info($"{player.GetPlayerIdentifier()} selected {packs.Count} pack(s).");
        
        await new ClientboundConfigurationRegistryDataPacket().Call(context, null);
        //new ClientboundConfigurationFinishPacket().Call(context, null); 
    }
}