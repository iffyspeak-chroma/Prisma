using API.Logging;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        // Start by checking if the server has run before
        if (FirstLaunch())
        {
            // So the server hasn't been run before. We should do the first setups before continuing.
            Setup();
        }

        if (RequiredFileCheck() != 0)
        {
            LogTool.Error("Missing required files in configuration folder!");
            Environment.Exit(RequiredFileCheck());
        }
    }
    
    static bool FirstLaunch()
    {
        if (Directory.Exists(Constants.ConfigurationDirectory))
        {
            // It likely has been launched before and should be okay to continue. Without any further setup
            return false;
        }

        return true;
    }

    static void Setup()
    {
        try
        {
            // Create all necessary directories
            // For now, this is technically the only one we'll need
            Directory.CreateDirectory(Constants.ConfigurationDirectory);
        }
        finally
        {
            LogTool.Info("Because this is your first time running Prisma: you should configure your server then launch Prisma again.");
            LogTool.Info("Exiting!");
            Environment.Exit(0);
        }
    }

    static int RequiredFileCheck()
    {
        const int configFile    = 0b00000001;
        const int packetsReport = 0b00000010;
        
        // Dependent on what's missing, it'll return that code.
        return (File.Exists(Constants.ConfigurationFile) ? 0 : configFile) | (File.Exists(Constants.PacketReportFile) ? 0 : packetsReport);
    }
}