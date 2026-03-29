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
    
    // (root) -> data
    public static string DataDirectory = Path.Join(ExecutingDirectory, "data");
    
    // (root) -> data -> minecraft
    public static string MinecraftDataDirectory = Path.Join(DataDirectory, "minecraft");
    
    // (root) -> data -> custom
    public static string CustomDataDirectory = Path.Join(DataDirectory, "custom");

    #endregion
    
    #region Files
    
    // (root) -> config -> config.json
    public static string ConfigurationFile = Path.Combine(ConfigurationDirectory, "config.json");
    
    // (root) -> data -> minecraft -> reports -> packets.json
    public static string PacketReportFile = Path.Combine(MinecraftDataDirectory, "reports", "packets.json");
    
    // (root) -> version -> server.jar
    public static string PackedServerFile = Path.Combine(VersionDirectory, "server.jar");

    #endregion

}