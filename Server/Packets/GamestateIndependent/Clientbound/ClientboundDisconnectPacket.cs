using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.GamestateIndependent.Clientbound;

public class ClientboundDisconnectPacket : ICallable
{
    public TextComponentBuilder DisconnectMessage = new TextComponentBuilder();
    public async void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
            
        packet.Write(DisconnectMessage.Build());

        switch (client.Gamestate)
        {
            case PlayerGamestate.Login:
            {
                packet.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_disconnect"].Id);
                packet.WriteLength();
                
                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
                break;
            }

            case PlayerGamestate.Configuration:
            {
                packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:disconnect"].Id);
                packet.WriteLength();
                
                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
                break;
            }

            case PlayerGamestate.Play:
            {
                packet.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:disconnect"].Id);
                packet.WriteLength();

                await PlayerManager.Instance.ConnectedClients[context.Channel].SendPacket(packet);
                break;
            }

            default:
            {
                throw new NotImplementedException();
            }
        }
        
        PlayerManager.Instance.ConnectedClients[context.Channel].DisconnectChannel();
        PlayerManager.Instance.ConnectedClients.Remove(context.Channel);
    }
    
    public async void Call(NetworkedClient client, Packet? packet)
    {
        packet.Write(DisconnectMessage.Build());

        switch (client.Gamestate)
        {
            case PlayerGamestate.Login:
            {
                packet.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_disconnect"].Id);
                packet.WriteLength();
                
                await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(packet);
                break;
            }

            case PlayerGamestate.Configuration:
            {
                packet.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:disconnect"].Id);
                packet.WriteLength();
                
                await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(packet);
                break;
            }

            default:
            {
                throw new NotImplementedException();
            }
        }
        
        PlayerManager.Instance.ConnectedClients[client.Channel].DisconnectChannel();
        PlayerManager.Instance.ConnectedClients.Remove(client.Channel);
    }
}