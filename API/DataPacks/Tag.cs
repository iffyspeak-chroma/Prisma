using API.Protocol.Mojang;

namespace API.DataPacks;

public class Tag
{
    public Identifier Id { get; init; }
    public List<TagValue> Values { get; init; } = new();

    public HashSet<Identifier> Resolved { get; set; } = new();
}