namespace API.DataTypes.Player;

public class ServerPlayer
{
    public string Username { get; set; } = "";
    public Guid Uuid { get; set; } = Guid.Empty;
    
    public ServerPlayer() {}

    public ServerPlayer(string u, Guid gu)
    {
        this.Username = u;
        this.Uuid = gu;
    }
}