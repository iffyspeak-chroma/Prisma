using API.Entities;

namespace API.Core.Managers;

public class EntityManager
{
    public static readonly EntityManager Instance = new EntityManager();

    public static int RetrieveNextId()
    {
        if (Instance.EntityList.Count <= 0)
        {
            return 0;
        }

        // Check for empty hole IDs, such as deleted entities
        for (int i = 0; i <= Instance.EntityList.Count; i++)
        {
            if (!Instance.EntityList.ContainsKey(i))
            {
                return i;
            }
        }

        return (Instance.EntityList.Count + 1);
    }
    
    public int GlobalTeleportId = 0;

    public Dictionary<int, GenericEntity> EntityList = new Dictionary<int, GenericEntity>();
}