namespace API.Player.Cosmetics;

[Flags]
public enum SkinParts : byte
{
    Cape = 0x01,
    Jacket = 0x02,
    LeftSleeve = 0x04,
    RightSleeve = 0x08,
    LeftPant = 0x10,
    RightPant = 0x20,
    Hat = 0x40,
    Unused = 0x80,
}