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
        
        using (Packet p = new Packet())
        {
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
            p.Write(actionByte);
            
            // Player count
            p.Write(AffectedPlayers.Count);

            //Player actions
            foreach (KeyValuePair<NetworkedClient, List<IPlayerActionFlag>> kvp in AffectedPlayers)
            {
                NetworkedClient kvClient = kvp.Key;
                List<IPlayerActionFlag> flags = kvp.Value;

                ServerPlayer player = kvClient.Player;
                
                p.Write(player.Uuid);
                foreach (IPlayerActionFlag flag in flags)
                {
                    p.Write(flag.GetData().ToArray());
                }
            }
            
            p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:player_info_update"].Id);
            p.WriteLength();

            await client.SendPacket(p);
            
            await new ClientboundPlayInitializeBorderPacket().Call(context, null);
        }
    }
}