using API.Entities;
using API.Player.Settings;

namespace API.Player;

public class ServerPlayer
{
    public string Username { get; set; } = "";
    public Guid Uuid { get; set; } = Guid.Empty;
    public ClientSettings? ClientSettings = null;
    public Dictionary<string, object?> PluginMessages { get; set; } = new Dictionary<string, object?>()
    {
        {"minecraft:brand", null}
    };

    public GenericEntity? AssociatedEntity = null;

    public int RecentTeleportId = -1;
    public bool AcceptMovementPackets = true;
    
    public ServerPlayer() {}

    public ServerPlayer(string u, Guid gu)
    {
        this.Username = u;
        this.Uuid = gu;
    }
    
    public string GetPlayerIdentifier()
    {
        return $"{Username}[{Uuid.ToString()}]";
    }
}