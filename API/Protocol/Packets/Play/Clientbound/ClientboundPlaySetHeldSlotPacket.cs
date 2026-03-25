using API.Core.Managers;
using API.Player.Actions.PlayerActionFlags;
using API.Player.State;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Play.Clientbound;

public class ClientboundPlaySetHeldSlotPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        if (packet == null)
        {
            packet = new Packet();
        }
        else
        {
            packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_held_slot"].Id);
            packet.WriteLength();

            await client.SendPacket(packet);
            
            return;
        }
        
        // TODO: Proper implementation
        // But for now we'll just use the date.
        DateTime date = new DateTime();
        int heldSlot = (date.Day % 8);
            
        packet.Write(heldSlot);
            
        packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:set_held_slot"].Id);
        packet.WriteLength();

        await client.SendPacket(packet);
        
        await new ClientboundPlaySynchronizePlayerPositionPacket().Call(context, null);

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
        await otherPlayersInfoPacket.Call(context, null);

        ClientboundPlayPlayerInfoUpdatePacket currentPlayerInfoPacket = new ClientboundPlayPlayerInfoUpdatePacket();
        currentPlayerInfoPacket.AffectedPlayers.Add(client, new List<IPlayerActionFlag>()
        {
            new AddPlayerAction(client.Player)
        });
        await currentPlayerInfoPacket.Call(context, null);
    }
}