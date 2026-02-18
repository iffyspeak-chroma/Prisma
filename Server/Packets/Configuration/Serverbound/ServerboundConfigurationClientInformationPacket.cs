using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Players;

namespace Server.Packets.Configuration.Serverbound;

public class ServerboundConfigurationClientInformationPacket : ICallable
{
    public void Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;
        
        ClientSettings cs = new ClientSettings();
        
        cs.Locale = packet.ReadString();
        cs.ViewDistance = packet.ReadByte();
        cs.ChatMode = (ClientChatMode) packet.ReadVarInt();
        cs.UseColors = packet.ReadBool();
        cs.DisplayedSkinPieces = (SkinParts) packet.ReadByte();
        cs.DominantHand = (DominantHand) packet.ReadVarInt();
        cs.EnableTextFiltering = packet.ReadBool();
        cs.ShowInServerList = packet.ReadBool();
        cs.ParticlePreference = (ParticlePreference) packet.ReadVarInt();

        client.Player.ClientSettings = cs;
        LogTool.Debug($"{PlayerManager.GetPlayerIdentifier(player)} uses {cs.Locale} and draws {cs.ViewDistance} chunks away.", true);
    }
}