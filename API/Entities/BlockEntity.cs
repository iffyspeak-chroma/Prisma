using fNbt;

namespace API.Entities;

public abstract class BlockEntity
{
    public int Id;
    public int X;
    public short Y;
    public int Z;
    public NbtCompound? Data;
}