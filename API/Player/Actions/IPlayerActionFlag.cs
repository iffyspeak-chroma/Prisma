using API.Protocol.Packets;

namespace API.Player.Actions.PlayerActionFlags;

public interface IPlayerActionFlag
{
    byte GetMask();
    Packet GetData();
}