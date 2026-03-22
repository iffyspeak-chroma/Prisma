using API.DataTypes;
using API.DataTypes.Mojang;

namespace API.World;

public class World
{
    public Identifier Identifier { get; private set; }
    public Dimension Dimension { get; private set; }
    public Location Spawn { get; private set; }

    public World(Identifier id, Dimension dim, Location loc)
    {
        Identifier = id;
        Dimension = dim;
        Spawn = loc;
    }

    public World(Identifier id, Dimension dim)
    {
        Identifier = id;
        Dimension = dim;
        Spawn = new Location(0, 64, 0);
    }

    public void SetWorldSpawn(Location location)
    {
        Spawn = location;
    }
}