using API.Protocol.Mojang;
using fNbt;

namespace API.DataPacks;

public class RegistryEntry
{
    public Identifier EntryId { get; init; }
    public NbtCompound? Data { get; init; }
}