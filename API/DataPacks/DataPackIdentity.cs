using API.Protocol.Mojang;

namespace API.DataPacks;

public class DataPackIdentity
{
    public string Namespace { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }

    public DataPackIdentity(string ns, string id, string version)
    {
        this.Namespace = ns;
        this.Id = id;
        this.Version = version;
    }

    public DataPackIdentity(Identifier identifier, string version)
    {
        this.Namespace = identifier.Namespace;
        this.Id = identifier.Value;
        this.Version = version;
    }
}