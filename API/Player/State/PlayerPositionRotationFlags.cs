namespace API.Player.State;

[Flags]
public enum PlayerPositionRotationFlags : byte
{
    Grounded = 0x01,
    AgainstWall = 0x02
}