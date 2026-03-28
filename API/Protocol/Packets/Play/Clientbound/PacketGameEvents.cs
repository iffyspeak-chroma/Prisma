namespace API.Protocol.Packets.Play.Clientbound;

public class PacketGameEvents
{
    public static byte InvalidRespawnBlock = 0;

    public static byte StartRain = 1;
    public static byte EndRain = 2;
    public static byte AdjustRainLevel = 7;
    public static byte AdjustThunderLevel = 8;

    public static byte ChangeGameMode = 3;

    public static byte WinGame = 4;

    public static byte DemoEvent = 5;

    public static byte ArrowHitPlayer = 6;
    public static byte PufferfishSting = 9;
    public static byte ElderGuardianCurse = 10;

    public static byte RespawnScreen = 11;
    public static byte LimitCrafting = 12;

    public static byte WaitForChunks = 13;
}