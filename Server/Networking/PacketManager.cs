using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Configuration.Serverbound;
using Server.Packets.Handshake;
using Server.Packets.Login.Serverbound;
using Server.Packets.Status.Serverbound;
using Server.Players;

namespace Server.Networking;

public class PacketManager
{
    public static readonly PacketManager Instance = new PacketManager();
    
    public delegate void PacketHandler(IChannelHandlerContext context, Packet packet);
    public Dictionary<PlayerGamestate, Dictionary<int, PacketHandler>> PacketList = new();

    public void Handle(IChannelHandlerContext context, Packet packet)
    {
        int packetId = packet.ReadVarInt();
        PacketList[PlayerManager.Instance.ConnectedClients[context.Channel].Gamestate][packetId].Invoke(context, packet);
    }

    public void ReceivedPacket(IChannelHandlerContext context, Packet packet)
    {
        if (!PlayerManager.Instance.ConnectedClients.ContainsKey(context.Channel))
        {
            // This is a brand new connecting client. Add them onto our list.
            PlayerManager.Instance.ConnectedClients.Add(context.Channel, 
                new NetworkedClient(PlayerGamestate.Handshake, context.Channel, new PlayerConnectionInfo()));

            HandleDecompressedPacket(context, packet);

            return;
        }

        if (!PlayerManager.Instance.ConnectedClients[context.Channel].IsCompressing)
        {
            // Don't decompress, the client and server haven't agreed to just yet.
            
            HandleDecompressedPacket(context, packet);
            return;
        }
        
        // Decompress the packet, the client and server have agreed!
        throw new NotImplementedException("Oopsies! Compression or Decompression have yet to be implemented..");
    }

    private void HandleDecompressedPacket(IChannelHandlerContext context, Packet packet)
    {
        try
        {
            Handle(context, packet);
        }
        catch (Exception exception)
        {
            LogTool.Exception(exception);
        }
    }

    public void InitializePacketList()
    {
        PlayerGamestate tState = PlayerGamestate.Handshake;
        
        #region Handshake
        tState = PlayerGamestate.Handshake;

        PacketList.Add(tState, new Dictionary<int, PacketHandler>());
        
        PacketList[tState].Add(PacketReport.Mapping.Handshake.Serverbound["minecraft:intention"].Id, new ServerboundHandshakePacket().Call);

        #endregion

        #region Status
        tState = PlayerGamestate.Status;

        PacketList.Add(tState, new Dictionary<int, PacketHandler>());
        
        PacketList[tState].Add(PacketReport.Mapping.Status.Serverbound["minecraft:status_request"].Id, new ServerboundStatusRequestPacket().Call);
        PacketList[tState].Add(PacketReport.Mapping.Status.Serverbound["minecraft:ping_request"].Id, new ServerboundPingRequestPacket().Call);

        #endregion

        #region Login
        tState = PlayerGamestate.Login;

        PacketList.Add(tState, new Dictionary<int, PacketHandler>());
        
        PacketList[tState].Add(PacketReport.Mapping.Login.Serverbound["minecraft:hello"].Id, new ServerboundLoginStartPacket().Call);
        PacketList[tState].Add(PacketReport.Mapping.Login.Serverbound["minecraft:login_acknowledged"].Id, new ServerboundLoginAcknowledgePacket().Call);

        #endregion
        
        #region Configuration
        tState = PlayerGamestate.Configuration;
        
        PacketList.Add(tState, new Dictionary<int, PacketHandler>());
        
        PacketList[tState].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:client_information"].Id, 
            new ServerboundConfigurationClientInformationPacket().Call);
        PacketList[tState].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:custom_payload"].Id,
            new ServerboundConfigurationPluginMessagePacket().Call);
        PacketList[tState].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:select_known_packs"].Id,
            new ServerboundConfigurationKnownPacksPacket().Call);
        #endregion
    }
}