using System.Numerics;

namespace API.Core;

public class ExtendedMath
{
    public static int CeilLog2(int x)
    {
        if (x <= 1) return 0;
        return BitOperations.Log2((uint)(x - 1)) + 1;
    }
}