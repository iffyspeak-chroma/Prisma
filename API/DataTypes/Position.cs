namespace API.DataTypes;

public class Position
{
    public int X;
    public int Y;
    public int Z;

    public Position(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Position(Location location)
    {
        X = (int) location.X;
        Y = (int) location.Y;
        Z = (int) location.Z;
    }

    public Position()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }
}