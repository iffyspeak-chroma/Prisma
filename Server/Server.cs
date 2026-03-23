using API.Networking;
using Server.Tools;

namespace Server;

public class Server
{
    public static Server? Instance = null;

    public ConfigMapping? Configuration = null;
    public PacketReport? PacketReport = null;
}