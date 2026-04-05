using System.Numerics;

namespace API.Core;

public class ExtendedMath
{
    public static int CeilLog2(int x)
    {
        if (x <= 1) return 0;
        return BitOperations.Log2((uint)(x - 1)) + 1;
    }

    public static float Percentage(int min, int max, int t)
    {
        if (max == min) return 0f;

        float pct = (t - min) / (float)(max - min);

        return Math.Clamp(pct, 0f, 1f);
    }

    public static float Scale(float rMin, float rMax, float rT, float sMin, float sMax)
    {
        float pct = (rT - rMin) / (rMax - rMin);
        
        return sMin + pct * (sMax - sMin);
    }
}