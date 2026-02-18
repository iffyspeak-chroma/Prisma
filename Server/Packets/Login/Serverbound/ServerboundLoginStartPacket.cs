using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using API.TextComponents;
using DotNetty.Transport.Channels;
using Server.Packets.Login.Clientbound;
using Server.Managers;

namespace Server.Packets.Login.Serverbound;

public class ServerboundLoginStartPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        string playerName = packet.ReadString();
        Guid playerId = packet.ReadGuid();

        PlayerManager.Instance.ConnectedClients[context.Channel].Player = new ServerPlayer(playerName, playerId);
        
        LogTool.Info($"{playerName}[{playerId.ToString()}] is attempting to connect to the server.");
        
        // TODO: Cryptography and Compression
        // TODO: Check if theres even space for a player or if the player is allowed to join while the server is full.
        // TODO: Check for duplicates of a player.
        using (Packet p = new Packet())
        {
            // Write in the player's "GameProfile" (https://minecraft.wiki/w/Java_Edition_protocol/Packets#Game_Profile)
            p.Write(playerId); // UUID
            p.Write(playerName); // User
            p.Write(0); // Properties
            
            new ClientboundLoginSuccessPacket().Call(context, p);
        }
    }
}