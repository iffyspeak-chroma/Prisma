using API.DataTypes.DataPacks;
using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationKnownPacksPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        List<DataPackIdentity> packs = new List<DataPackIdentity>();
        
        int packCount = packet.ReadVarInt();
        for (int i = packCount; i > 0; i--)
        {
            DataPackIdentity next = new  DataPackIdentity();
            next.Namespace = packet.ReadString();
            next.ID = packet.ReadString();
            next.Version = packet.ReadString();
            
            packs.Add(next);
        }
        
        LogTool.Info($"{NetworkedClient.GetPlayerIdentifier(client.Player)} selected {packs.Count} pack(s).");
    }
}