using API.Networking;
using Server.ConfigurationTools;

namespace Server;

public class Server
{
    public static Server? Instance = null;

    public ConfigMapping? Configuration = null;
    public PacketReport? PacketReport = null;
}