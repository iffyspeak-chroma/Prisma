using API.Networking;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace API.DataTypes.Player;

public class NetworkedClient
{
    public bool IsConnected { get; set; }
    public bool IsCompressing { get; set; }
    public bool IsEncrypting { get; set; }
    public IChannel Channel { get; set; }
    public PlayerGamestate Gamestate { get; set; }
    
    public PlayerConnectionInfo PlayerConnectionInfo { get; set; }

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

    public async Task SendPacket(Packet packet)
    {
        var buffer = Unpooled.WrappedBuffer(packet.ToArray());
        await Channel.WriteAndFlushAsync(buffer);
    }
}