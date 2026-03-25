namespace API.Core;

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

    #region Varied Variables

    public static int GlobalTeleportId = 0;

    #endregion

    #region Colors

    public static string ErrorColorPrimary = "#eb251e";
    public static string ErrorColorSecondary = "#e65c57";

    public static string WarningColorPrimary = "#ebeb1e";
    public static string WarningColorSecondary = "#e6e357";

    public static string SuccessColorPrimary = "#25eb1e";
    public static string SuccessColorSecondary = "#5ce657";

    #endregion

}