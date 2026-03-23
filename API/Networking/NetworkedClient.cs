using API.DataTypes.Player;
using API.Networking;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace API.Networking;

public class NetworkedClient
{
    public bool IsConnected { get; set; }
    public bool IsCompressing { get; set; }
    public bool IsEncrypting { get; set; }
    public IChannel Channel { get; set; }
    public PlayerGamestate Gamestate { get; set; }
    public PlayerConnectionInfo PlayerConnectionInfo { get; set; }
    public ServerPlayer Player { get; set; }

    public static string GetPlayerIdentifier(ServerPlayer player)
    {
        return $"{player.Username}[{player.Uuid.ToString()}]";
    }
    
    public NetworkedClient(IChannel channel, PlayerConnectionInfo pci)
    {
        Channel = channel;
        PlayerConnectionInfo = pci;
        IsConnected = false;
        IsCompressing = false;
        IsEncrypting = false;
    }

    public NetworkedClient(PlayerGamestate gamestate, IChannel channel, PlayerConnectionInfo pci)
    {
        IsConnected = false;
        IsCompressing = false;
        IsEncrypting = false;
        Gamestate = gamestate;
        Channel = channel;
        PlayerConnectionInfo = pci;
    }

    public Task SendPacket(Packet packet)
    {
        if (!Channel.Active)
            return Task.CompletedTask;

        var buffer = Unpooled.WrappedBuffer(packet.ToArray());
        return Channel.WriteAndFlushAsync(buffer);
    }

    public void DisconnectChannel()
    {
        Channel.CloseAsync();
    }
}