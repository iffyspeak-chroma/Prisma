using API.World;

namespace Server.Managers;

public class WorldManager
{
    public static readonly WorldManager Instance = new WorldManager();

    public List<World> Worlds;

    public WorldManager()
    {
        Worlds = new();
    }
}