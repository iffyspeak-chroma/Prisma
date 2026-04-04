using fNbt;

namespace API.Entities;

public abstract class BlockEntity
{
    public int Id;
    public int X;
    public int Y;
    public int Z;
    public NbtCompound? Data;
}