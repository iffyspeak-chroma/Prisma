using API.DataTypes.Mojang;

namespace API.DataTypes.DataPacks;

public class DataPackIdentity
{
    public string Namespace { get; set; }
    public string ID { get; set; }
    public string Version { get; set; }

    public DataPackIdentity(string ns, string id, string version)
    {
        this.Namespace = ns;
        this.ID = id;
        this.Version = version;
    }

    public DataPackIdentity(Identifier identifier, string version)
    {
        this.Namespace = identifier.Namespace;
        this.ID = identifier.Value;
        this.Version = version;
    }
}