using API.DataTypes.Player;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.GamestateIndependent.Clientbound;

public class ClientboundDisconnectPacket : ICallable
{
    public TextComponentBuilder DisconnectMessage = new TextComponentBuilder();
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
            
        DoDisconnect(client);
    }
    
    public void Call(NetworkedClient client, Packet? packet)
    {
        DoDisconnect(client);
    }

    private async void DoDisconnect(NetworkedClient client)
    {
        using (Packet p = new Packet())
        {
            p.Write(DisconnectMessage.Build());

            switch (client.Gamestate)
            {
                case PlayerGamestate.Login:
                {
                    p.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }

                case PlayerGamestate.Configuration:
                {
                    p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }
            
                case PlayerGamestate.Play:
                {
                    p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:disconnect"].Id);
                    p.WriteLength();

                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
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
}