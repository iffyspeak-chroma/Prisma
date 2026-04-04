namespace API.Game;

[Flags]
public enum TeleportFlags
{
    None = 0,
    
    RelativeX          = 0x0001,
    RelativeY          = 0x0002,
    RelativeZ          = 0x0004,
    RelativeYaw        = 0x0008,
    RelativePitch      = 0x0010,

    RelativeVelocityX  = 0x0020,
    RelativeVelocityY  = 0x0040,
    RelativeVelocityZ  = 0x0080,
    
    RotateVelocity     = 0x0100
}