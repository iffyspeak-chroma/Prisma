using API.Core.Managers;
using API.Player;
using API.Player.Cosmetics;
using API.Player.Settings;
using API.Protocol.Networking;
using API.Protocol.Packets.Configuration.Clientbound;
using DotNetty.Transport.Channels;

namespace API.Protocol.Packets.GamestateIndependent.Serverbound;

public class ServerboundClientInformationPacket : ICallablePacket
{
    public async Task Call(IChannelHandlerContext context, Packet? packet)
    {
        NetworkedClient client = PlayerManager.Instance.ConnectedClients[context.Channel];
        ServerPlayer player = client.Player;

        if (packet == null)
            return;
        
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
        //LogTool.Debug($"{NetworkedClient.GetPlayerIdentifier(player)} uses {cs.Locale} and draws {cs.ViewDistance} chunks away.");

        await new ClientboundConfigurationPluginMessagePacket().Call(context, null);
    }
}