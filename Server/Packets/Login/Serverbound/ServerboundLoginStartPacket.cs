using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Packets.Login.Clientbound;
using Server.Players;

namespace Server.Packets.Login.Serverbound;

public class ServerboundLoginStartPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        string playerName = packet.ReadString();
        Guid playerId = packet.ReadGuid();

        PlayerManager.Instance.ConnectedClients[context.Channel].Player = new ServerPlayer(playerName, playerId);
        
        LogTool.Info($"{playerName}[{playerId.ToString()}] is attempting to connect to the server.");
        
        // TODO: Replace this with the actual next step.
        using (Packet p = new Packet())
        {
            // I'm just going to disconnect them for now
            TextComponentBuilder builder = new TextComponentBuilder();

            builder.AddText("Uh oh!\n", color: "red");
            builder.AddText("I'm not quite ready for that kinda thing just yet.");
            
            p.Write(builder.Build());

            new ClientboundLoginDisconnectPacket().Call(context, p);
        }
    }
}