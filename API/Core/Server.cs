using API.Protocol.Packets;

namespace API.Core;

public class Server
{
    public static Server? Instance = null;

    public ServerConfiguration? Configuration = null;
    public PacketReport? PacketReport = null;
}