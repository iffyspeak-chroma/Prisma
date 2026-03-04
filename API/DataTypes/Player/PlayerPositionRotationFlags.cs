namespace API.DataTypes.Player;

[Flags]
public enum PlayerPositionRotationFlags : byte
{
    Grounded = 0x01,
    AgainstWall = 0x02
}