namespace API.DataTypes.Entities;

public class GenericEntity
{
    // TODO: Populate this
    public Location Location;
    public Location Velocity;
    public Heading LookDirection;

    public GenericEntity()
    {
        Location = new Location();
        Velocity = new Location();
    }
}