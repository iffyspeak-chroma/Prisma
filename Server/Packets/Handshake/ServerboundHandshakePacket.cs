using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Handshake;

public class ServerboundHandshakePacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            throw new NullReferenceException("Packet is null where it shouldn't be.");
        }
        
        int protocolVersion = packet.ReadVarInt();
        string connectionAddress = packet.ReadString();
        ushort connectionPort = (ushort) packet.ReadShort(flipped: true);
        PlayerGamestate nextIntent = (PlayerGamestate)packet.ReadVarInt();
        
        PlayerConnectionInfo pci = new  PlayerConnectionInfo(protocolVersion, connectionAddress,  connectionPort);

        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        client.Gamestate = nextIntent;
        client.PlayerConnectionInfo = pci;
    }
}