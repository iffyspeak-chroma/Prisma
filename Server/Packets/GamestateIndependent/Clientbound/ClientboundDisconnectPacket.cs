using API.DataTypes.Player;
using API.NBT;
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
            switch (client.Gamestate)
            {
                // JSON Text Component
                case PlayerGamestate.Login:
                {
                    p.Write(DisconnectMessage.ToJson());
                    p.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }

                // NBT Text Component
                case PlayerGamestate.Configuration:
                {
                    throw new NotImplementedException();
                    //p.Write(NbtUtility.ConvertToBytes(DisconnectMessage.ToNetworkNbt()));
                    p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }
            
                // NBT Text Component
                case PlayerGamestate.Play:
                {
                    throw new NotImplementedException();
                    //p.Write(NbtUtility.ConvertToBytes(DisconnectMessage.ToNetworkNbt()));
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