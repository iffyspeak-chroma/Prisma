using API.DataTypes.Entities;

namespace API.DataTypes.Player;

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
    
    public ServerPlayer() {}

    public ServerPlayer(string u, Guid gu)
    {
        this.Username = u;
        this.Uuid = gu;
    }
}