namespace Server;

public class Constants
{
    #region Directories

    // (root)
    public static readonly string ExecutingDirectory = Environment.CurrentDirectory;
    
    // (root) -> config
    public static readonly string ConfigurationDirectory = Path.Join(ExecutingDirectory, "config");
    
    // (root) -> version
    public static readonly string VersionDirectory = Path.Join(ExecutingDirectory, "version");
    
    // (root) -> packdata
    public static readonly string PackDataDirectory = Path.Join(ExecutingDirectory, "packdata");
    
    // (root) -> packdata -> vanilla
    public static readonly string VanillaDirectory = Path.Join(PackDataDirectory, "vanilla");
    
    // (root) -> packdata -> custom
    public static readonly string CustomDirectory = Path.Join(PackDataDirectory, "custom");

    #endregion
    
    #region Files
    
    // (root) -> config -> config.json
    public static readonly string ConfigurationFile = Path.Combine(ConfigurationDirectory, "config.json");
    
    // (root) -> packdata -> vanilla -> reports -> packets.json
    public static readonly string PacketReportFile = Path.Combine(VanillaDirectory, "reports", "packets.json");
    
    // (root) -> packdata -> vanilla -> reports -> registries.json
    public static readonly string RegistriesReportFile = Path.Combine(VanillaDirectory, "reports", "registries.json");
    
    // (root) -> version -> server.jar
    public static readonly string MojangServerFile = Path.Combine(VersionDirectory, "server.jar");

    #endregion

}