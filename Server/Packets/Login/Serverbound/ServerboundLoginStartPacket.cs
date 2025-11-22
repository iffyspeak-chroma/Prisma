using API.DataTypes.Player;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Login.Serverbound;

public class ServerboundLoginStartPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        string playerName = packet.ReadString();
        Guid playerId = packet.ReadGuid();

        PlayerManager.Instance.ConnectedClients[context.Channel].Player = new ServerPlayer(playerName, playerId);
    }
}