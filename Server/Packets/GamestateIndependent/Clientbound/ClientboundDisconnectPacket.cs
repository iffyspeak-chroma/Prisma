using API.DataTypes.Player;
using API.Logging;
using API.NBT;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using fNbt;
using Server.Managers;

namespace Server.Packets.GamestateIndependent.Clientbound;

public class ClientboundDisconnectPacket : ICallable
{
    public TextComponentBuilder DisconnectMessage = new TextComponentBuilder();
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
            
        await DoDisconnect(client);
    }
    
    public async Task Call(NetworkedClient client, Packet? packet)
    {
        await DoDisconnect(client);
    }

    private async Task DoDisconnect(NetworkedClient client)
    {
        using (Packet p = new Packet())
        {
            switch (client.Gamestate)
            {
                case PlayerGamestate.Login:
                {
                    p.Write(DisconnectMessage.ToJson());
                    p.InsertInt(PacketReport.Mapping.Login.Clientbound["minecraft:login_disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }
                
                case PlayerGamestate.Configuration:
                {
                    p.Write(NbtToolkit.StripUnnecessary(DisconnectMessage.ToNbtFile().SaveToBuffer(NbtCompression.None)));
                    p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:disconnect"].Id);
                    p.WriteLength();
                
                    await PlayerManager.Instance.ConnectedClients[client.Channel].SendPacket(p);
                    break;
                }

                case PlayerGamestate.Play:
                {
                    p.Write(NbtToolkit.StripUnnecessary(DisconnectMessage.ToNbtFile().SaveToBuffer(NbtCompression.None)));
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