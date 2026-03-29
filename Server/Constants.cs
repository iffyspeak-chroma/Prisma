namespace Server;

public class Constants
{
    #region Directories

    // (root)
    public static string ExecutingDirectory = Environment.CurrentDirectory;
    
    // (root) -> config
    public static string ConfigurationDirectory = Path.Join(ExecutingDirectory, "config");
    
    // (root) -> version
    public static string VersionDirectory = Path.Join(ExecutingDirectory, "version");
    
    // (root) -> packdata
    public static string PackDataDirectory = Path.Join(ExecutingDirectory, "packdata");
    
    // (root) -> packdata -> minecraft
    public static string MinecraftDataDirectory = Path.Join(PackDataDirectory, "minecraft");
    
    // (root) -> packdata -> custom
    public static string CustomDataDirectory = Path.Join(PackDataDirectory, "custom");

    #endregion
    
    #region Files
    
    // (root) -> config -> config.json
    public static string ConfigurationFile = Path.Combine(ConfigurationDirectory, "config.json");
    
    // (root) -> packdata -> minecraft -> reports -> packets.json
    public static string PacketReportFile = Path.Combine(MinecraftDataDirectory, "reports", "packets.json");
    
    // (root) -> version -> server.jar
    public static string PackedServerFile = Path.Combine(VersionDirectory, "server.jar");

    #endregion

}