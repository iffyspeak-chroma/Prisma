using API.Core.Managers;
using API.Player;
using API.Player.Actions.PlayerActionFlags;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlayPlayerInfoUpdatePacket : ICallablePacket
{
    public Dictionary<NetworkedClient, List<IPlayerActionFlag>> AffectedPlayers = new();
        
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_info_update"].Id);
            packet.WriteLength();

            await client.SendPacket(packet);

            return;
        }
        
        byte actionByte = 0;

        foreach (KeyValuePair<NetworkedClient, List<IPlayerActionFlag>> kvp in AffectedPlayers)
        {
            List<IPlayerActionFlag> flags = kvp.Value;
                
            foreach (IPlayerActionFlag flag in flags)
            {
                actionByte |= flag.GetMask();
            }
        }
            
        // Actions field
        packet.Write(actionByte);
            
        // Player count
        packet.Write(AffectedPlayers.Count);

        //Player actions
        foreach (KeyValuePair<NetworkedClient, List<IPlayerActionFlag>> kvp in AffectedPlayers)
        {
            NetworkedClient kvClient = kvp.Key;
            List<IPlayerActionFlag> flags = kvp.Value;

            ServerPlayer player = kvClient.Player;
                
            packet.Write(player.Uuid);
            foreach (IPlayerActionFlag flag in flags)
            {
                packet.Write(flag.GetData().ToArray());
            }
        }
            
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_info_update"].Id);
        packet.WriteLength();

        await client.SendPacket(packet);
            
        await new ClientboundPlayInitializeBorderPacket().Call(context, null);
    }
}