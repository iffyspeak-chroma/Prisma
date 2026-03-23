using API.DataTypes.DataPacks;
using API.DataTypes.Mojang;
using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Configuration.Clientbound;
using Server.Managers;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationKnownPacksPacket : ICallable
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