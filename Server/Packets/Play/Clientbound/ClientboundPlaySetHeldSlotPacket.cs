using API.DataTypes.Player;
using API.DataTypes.Player.PlayerActionFlags;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.Play.Clientbound;

public class ClientboundPlaySetHeldSlotPacket : ICallable
{
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        
        using (Packet p = new Packet())
        {
            // TODO: Proper implementation
            // But for now we'll just use the date.
            DateTime date = new DateTime();
            int heldSlot = (date.Day % 8);
            
            p.Write(heldSlot);
            
            p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_held_slot"].Id);
            p.WriteLength();

            await client.SendPacket(p);
        }
        
        new ClientboundPlaySynchronizePlayerPositionPacket().Call(context, null);

        ClientboundPlayPlayerInfoUpdatePacket otherPlayersInfoPacket = new ClientboundPlayPlayerInfoUpdatePacket();
        foreach(KeyValuePair<IChannel, NetworkedClient> kvp in PlayerManager.Instance.ConnectedClients)
        {
            NetworkedClient otherClient = kvp.Value;
            
            if ((otherClient != client) && (otherClient.Gamestate == PlayerGamestate.Play))
            {
                otherPlayersInfoPacket.AffectedPlayers.Add(otherClient, new List<IPlayerActionFlag>()
                {
                    new AddPlayerAction(otherClient.Player)
                });
            }
        }
        otherPlayersInfoPacket.Call(context, null);

        ClientboundPlayPlayerInfoUpdatePacket currentPlayerInfoPacket = new ClientboundPlayPlayerInfoUpdatePacket();
        currentPlayerInfoPacket.AffectedPlayers.Add(client, new List<IPlayerActionFlag>()
        {
            new AddPlayerAction(client.Player)
        });
        currentPlayerInfoPacket.Call(context, null);

        // TODO: REMOVE THIS
        PlayerManager.DisconnectPlayer(client);
    }
}