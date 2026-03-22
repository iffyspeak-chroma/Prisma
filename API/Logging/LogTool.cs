namespace API.Logging;

public class LogTool
{
    public static void Info(string message)
    {
        Console.WriteLine($"Info || {DateTime.Now} >> {message}");
    }

    public static void Warn(string message)
    {
        ConsoleColor fgOriginal = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        
        Console.WriteLine($"Warning || {DateTime.Now} >> {message}");
        
        Console.ForegroundColor = fgOriginal;
    }

    public static void Error(string message)
    {
        ConsoleColor fgOriginal = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        
        Console.WriteLine($"Error || {DateTime.Now} >> {message}");
        
        Console.ForegroundColor = fgOriginal;
    }

    public static void Debug(string message)
    {
        ConsoleColor fgOriginal = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        
        Console.WriteLine($"Debug || {DateTime.Now} >> {message}");
        
        Console.ForegroundColor = fgOriginal;
    }

    public static void Raw(string message, ConsoleColor color = ConsoleColor.White)
    {
        ConsoleColor fgOriginal = Console.ForegroundColor;
        Console.ForegroundColor = color;
        
        Console.WriteLine($"{message}");
        
        Console.ForegroundColor = fgOriginal;
    }
    
    public static void RawDebug(string message, bool debug, ConsoleColor color = ConsoleColor.White)
    {
        if (debug)
        {
            Raw(message, color);
        }
    }

    public static void Exception(Exception exception)
    {
        Error($"An exception occurred: {exception.Message}");
        Raw($"{exception.StackTrace}", ConsoleColor.Red);
    }
}