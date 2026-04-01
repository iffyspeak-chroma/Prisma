using API.Protocol.Mojang;

namespace API.DataPacks;

public class RawTagEntry
{
    public Identifier Id { get; init; }
    public List<string> Values { get; } = new();
}