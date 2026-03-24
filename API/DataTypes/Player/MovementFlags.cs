namespace API.DataTypes.Player;

[Flags]
public enum MovementFlags : byte
{
    OnGround = 0x01,
    AgainstWall = 0x02,
}