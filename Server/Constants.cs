namespace Server;

public class Constants
{
    #region Directories

    public static string ExecutingDirectory = Environment.CurrentDirectory;
    public static string ConfigurationDirectory = Path.Join(ExecutingDirectory, "config");
    public static string VersionDirectory = Path.Join(ExecutingDirectory, "version");
    public static string GeneratedDataDirectory = Path.Join(ExecutingDirectory, "data");

    #endregion
    
    #region Files
    
    public static string ConfigurationFile = Path.Combine(ConfigurationDirectory, "config.json");
    public static string PacketReportFile = Path.Combine(GeneratedDataDirectory, "reports", "packets.json");
    
    public static string PackedServerFile = Path.Combine(VersionDirectory, "server.jar");

    #endregion

}