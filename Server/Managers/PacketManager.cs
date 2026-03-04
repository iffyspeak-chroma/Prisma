using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
using Server.Packets.Configuration.Serverbound;
using Server.Packets.Handshake;
using Server.Packets.Login.Serverbound;
using Server.Packets.Play.Serverbound;
using Server.Packets.Status.Serverbound;

namespace Server.Managers;

public class PacketManager
{
    public static readonly PacketManager Instance = new PacketManager();
    
    public delegate void PacketHandler(IChannelHandlerContext context, Packet packet);
    public Dictionary<PlayerGamestate, Dictionary<int, PacketHandler?>> PacketList = new();

    public void Handle(IChannelHandlerContext context, Packet packet)
    {
        int packetId = packet.ReadVarInt();
        PacketList[PlayerManager.Instance.ConnectedClients[context.Channel].Gamestate][packetId]?.Invoke(context, packet); 
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
        PlayerGamestate gamestate = PlayerGamestate.Handshake;
        
        #region Handshake
        gamestate = PlayerGamestate.Handshake;

        PacketList.Add(gamestate, new Dictionary<int, PacketHandler?>());
        
        PacketList[gamestate].Add(PacketReport.Mapping.Handshake.Serverbound["minecraft:intention"].Id, new ServerboundHandshakePacket().Call);

        #endregion

        #region Status
        gamestate = PlayerGamestate.Status;

        PacketList.Add(gamestate, new Dictionary<int, PacketHandler?>());
        
        PacketList[gamestate].Add(PacketReport.Mapping.Status.Serverbound["minecraft:status_request"].Id, new ServerboundStatusRequestPacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Status.Serverbound["minecraft:ping_request"].Id, new ServerboundPingRequestPacket().Call);

        #endregion

        #region Login
        gamestate = PlayerGamestate.Login;

        PacketList.Add(gamestate, new Dictionary<int, PacketHandler?>());
        
        PacketList[gamestate].Add(PacketReport.Mapping.Login.Serverbound["minecraft:hello"].Id, new ServerboundLoginStartPacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Login.Serverbound["minecraft:login_acknowledged"].Id, new ServerboundLoginAcknowledgePacket().Call);

        #endregion
        
        #region Configuration
        gamestate = PlayerGamestate.Configuration;
        
        PacketList.Add(gamestate, new Dictionary<int, PacketHandler?>());
        
        PacketList[gamestate].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:client_information"].Id, 
            new ServerboundConfigurationClientInformationPacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:custom_payload"].Id,
            new ServerboundConfigurationPluginMessagePacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:select_known_packs"].Id,
            new ServerboundConfigurationKnownPacksPacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Configuration.Serverbound["minecraft:finish_configuration"].Id,
            new ServerboundConfigurationAcknowledgeFinishPacket().Call);

        #endregion

        #region Play
        gamestate = PlayerGamestate.Play;
        
        PacketList.Add(gamestate, new Dictionary<int, PacketHandler?>());
        
        // Not every packet really needs a response I don't think
        // this will be the first instance of a client not getting a response about a packet.
        PacketList[gamestate].Add(PacketReport.Mapping.Play.Serverbound["minecraft:client_tick_end"].Id,
            null);
        PacketList[gamestate].Add(PacketReport.Mapping.Play.Serverbound["minecraft:accept_teleportation"].Id,
            new ServerboundPlayAcceptTeleportPacket().Call);
        PacketList[gamestate].Add(PacketReport.Mapping.Play.Serverbound["minecraft:move_player_pos_rot"].Id,
            new ServerboundPlaySetPlayerPosRotPacket().Call);

        #endregion
    }
}