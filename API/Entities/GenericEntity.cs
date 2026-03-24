using API.Core.Transform;

namespace API.Entities;

public class GenericEntity
{
    // TODO: Populate this
    public Location Location;
    public Location Velocity;
    public Heading LookDirection;
    public Guid Uid;

    public GenericEntity()
    {
        Location = new Location();
        Velocity = new Location();
        LookDirection = new Heading();
        Uid = Guid.NewGuid();
    }
    
    public GenericEntity(Guid uuid)
    {
        Location = new Location();
        Velocity = new Location();
        LookDirection = new Heading();
        Uid = uuid;
    }
}