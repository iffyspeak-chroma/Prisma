using Server.ConfigurationTools;
using API.Networking;

namespace Server;

public class Server
{
    public static Server? Instance = null;

    public ConfigMapping? Configuration = null;
    public PacketReport? PacketReport = null;
}