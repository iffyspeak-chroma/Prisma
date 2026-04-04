using API.Core.Transform;
using fNbt;

namespace API.Entities;

public class GenericEntity
{
    // TODO: Populate this
    public Location Location;
    public Location Velocity;
    public Heading LookDirection;
    public Guid Uid;
    public NbtCompound Data;

    public GenericEntity()
    {
        Location = new Location();
        Velocity = new Location();
        LookDirection = new Heading();
        Uid = Guid.NewGuid();
        Data = new NbtCompound("");
    }
    
    public GenericEntity(Guid uuid)
    {
        Location = new Location();
        Velocity = new Location();
        LookDirection = new Heading();
        Uid = uuid;
        Data = new NbtCompound("");
    }
}