namespace API.DataTypes.DataPacks;

public class DataPackIdentity
{
    public string Namespace { get; set; }
    public string ID { get; set; }
    public string Version { get; set; }
    
    public DataPackIdentity() {}
}