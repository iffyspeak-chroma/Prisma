using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Managers;

namespace Server.Packets.GamestateIndependent.Clientbound;

public class ClientboundKeepAlivePacket : ICallable
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];

        await Heartbeat(client);
    }

    public async Task Call(NetworkedClient client, Packet? packet)
    {
        await Heartbeat(client);
    }

    private async Task Heartbeat(NetworkedClient client)
    {
        using (Packet p = new Packet())
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            client.LastHeartbeat = timestamp;
            
            p.Write(timestamp, asVarLong: false);
            
            switch (client.Gamestate)
            {
                case PlayerGamestate.Configuration:
                {
                    p.InsertInt(PacketReport.Mapping.Configuration.Clientbound["minecraft:keep_alive"].Id);
                    p.WriteLength();

                    await client.SendPacket(p);
                    return;
                }
                case PlayerGamestate.Play:
                {
                    p.InsertInt(PacketReport.Mapping.Play.Clientbound["minecraft:keep_alive"].Id);
                    p.WriteLength();

                    await client.SendPacket(p);
                    return;
                }
                default:
                {
                    return;
                }
            }
        }
    }
}