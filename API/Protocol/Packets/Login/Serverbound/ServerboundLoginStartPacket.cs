using API.Core.Managers;
using API.Logging;
using API.Player;
using API.Protocol.Networking;
using API.Protocol.Packets.Login.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.Login.Serverbound;

public class ServerboundLoginStartPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        if (packet == null)
        {
            return;
        }
        
        string playerName = packet.ReadString();
        Guid playerId = packet.ReadGuid();

        PlayerManager.Instance.ConnectedClients[context.Channel].Player = new ServerPlayer(playerName, playerId);
        
        LogTool.Info($"{playerName}[{playerId.ToString()}] is attempting to connect to the server.");
        
        // TODO: Cryptography and Compression
        // TODO: Check if theres even space for a player or if the player is allowed to join while the server is full.
        // TODO: Check for duplicates of a player.
        using Packet p = new Packet();
        // Write in the player's "GameProfile" (https://minecraft.wiki/w/Java_Edition_protocol/Packets#Game_Profile)
        p.Write(playerId); // UUID
        p.Write(playerName); // User
        p.Write(0); // Properties
            
        await new ClientboundLoginSuccessPacket().Call(context, p);
    }
}