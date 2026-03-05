using API.Networking;

namespace API.DataTypes.Player.PlayerActionFlags;

public class AddPlayerAction : IPlayerActionFlag
{
    private ServerPlayer _player;
    
    public AddPlayerAction(ServerPlayer player)
    {
        this._player = player;
    }
    
    public byte GetMask()
    {
        return 0x01;
    }

    public Packet GetData()
    {
        Packet p = new Packet();

        p.Write(_player.Username);
            
        // We dont have any profile properties
        // TODO: Game Profile properties
        p.Write(0);
            
        return p;   
    }
}