namespace API.Protocol.Packets.Play.Clientbound;

public class PacketGameEventOptions
{
    public static class Generic
    {
        public static float Empty = 0.0f;
    }

    public static class ChangeGameMode
    {
        public static float Survival = 0.0f;
        public static float Creative = 1.0f;
        public static float Adventure = 2.0f;
        public static float Spectator = 3.0f;
    }

    public static class WinGame
    {
        public static float RespawnOnly = 0.0f;
        // CinemaSins reference
        public static float RollCredits = 1.0f;
    }

    public static class DemoEvent
    {
        public static float Welcome = 0.0f;
        public static float MovementControls = 101.0f;
        public static float JumpControl = 102.0f;
        public static float InventoryControl = 103.0f;
        public static float ScreenshotControl = 104.0f;
    }

    public static class RainChange
    {
        public static float Easing(float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
        }

        public static float Max = 1.0f;
        public static float Min = 0.0f;
    }

    public static class ThunderLevelChange
    {
        public static float Easing(float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
        }

        public static float Max = 1.0f;
        public static float Min = 0.0f;
    }

    public static class RespawnScreen
    {
        public static float Enabled = 0.0f;
        public static float Disabled = 1.0f;
    }

    public static class LimitedCrafting
    {
        public static float Disabled = 0.0f;
        public static float Enabled = 1.0f;
    }
}