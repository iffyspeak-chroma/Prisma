using API.DataTypes.Entities;

namespace Server.Managers;

public class EntityManager
{
    public static readonly EntityManager Instance = new EntityManager();

    public Dictionary<int, GenericEntity> EntityList = new Dictionary<int, GenericEntity>();
}