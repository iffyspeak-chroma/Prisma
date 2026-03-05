using API.Networking;

namespace API.DataTypes.Player.PlayerActionFlags;

public interface IPlayerActionFlag
{
    byte GetMask();
    Packet GetData();
}