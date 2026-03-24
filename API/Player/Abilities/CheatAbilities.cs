namespace API.Player.Abilities;

[Flags]
public enum CheatAbilities : byte
{
    Invulnerable = 0x01,
    Flying = 0x02,
    PermitFlying = 0x04,
    InstantBreak = 0x08
}