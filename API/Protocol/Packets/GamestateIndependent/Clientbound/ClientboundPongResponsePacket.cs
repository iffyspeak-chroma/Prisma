using API.Core.Managers;
using API.Player.State;
using API.Protocol.Networking;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.GamestateIndependent.Clientbound;

public class ClientboundPongResponsePacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            throw new InvalidOperationException("Packet should not be null in this context.");
        }
        
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        switch (client.Gamestate)
        {
            case PlayerGamestate.Status:
            {
                packet.InsertInt(PacketReport.Mapping.Status.Clientbound["minecraft:pong_response"].Id);
                packet.WriteLength();

                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
                
                // Since they aren't actually dead-set on playing just yet, we'll remove them from our list
                PlayerManager.Instance.ConnectedClients[context.Channel].DisconnectChannel();
                PlayerManager.Instance.ConnectedClients.Remove(context.Channel);
                break;
            }
            
            case PlayerGamestate.Play:
            {
                packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:pong_response"].Id);
                packet.WriteLength();

                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
                break;
            }
        }
    }
}