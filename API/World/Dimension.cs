using API.DataTypes.Mojang;

namespace API.World;

public class Dimension
{
    public Identifier Identifier { get; private set; }

    public Dimension(Identifier id)
    {
        Identifier = id;
    }
}