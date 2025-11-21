namespace Server;

public class Constants
{
    #region Directories

    public static string ExecutingDirectory = Environment.CurrentDirectory;
    public static string ConfigurationDirectory = Path.Join(ExecutingDirectory, "config");

    #endregion
    
    #region Files
    
    public static string ConfigurationFile = Path.Combine(ConfigurationDirectory, "config.json");
    public static string PacketReportFile = Path.Combine(ConfigurationDirectory, "packets.json");
    
    #endregion

}