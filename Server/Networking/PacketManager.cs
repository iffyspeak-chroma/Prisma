using API.DataTypes.Player;
using API.Logging;
using API.Networking;
using DotNetty.Transport.Channels;
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
        }
        
        // Decompress the packet, the client and server have agreed!
        throw new NotImplementedException("Oopsies! I haven't implemented compression or decompression.");
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
}