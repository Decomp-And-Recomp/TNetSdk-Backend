namespace TNet;

public enum LogLevel
{
    Info,
    Log,
    Warning,
    Error,
    Exception,
    Critical
}

public static class Logger
{
    private static readonly SemaphoreSlim LogLock = new(1, 1);

    public static void Log(LogLevel level, object? message)
    {
        _ = LogInner(level, message);
    }

    public static void Log(object message)
        => Log(LogLevel.Log, message);

    public static void Info(object message)
        => Log(LogLevel.Info, message);

    public static void Warning(object message)
        => Log(LogLevel.Warning, message);

    public static void Error(object message)
        => Log(LogLevel.Error, message);

    public static void Exception(Exception exception)
        => Log(LogLevel.Exception, $"Message: {exception.Message}\nStack Trace: {exception.StackTrace}");

    public static void Exception(Exception exception, object additional)
        => Log(LogLevel.Exception, $"Message: {exception.Message}\nStack Trace: {exception.StackTrace}\nAdditional:{additional}");

    private static async Task LogInner(LogLevel level, object? message)
    {
        await LogLock.WaitAsync();

        try
        {
            switch (level)
            {
                case LogLevel.Info:
                    Console.Write("Info      ");
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogLevel.Log:
                    Console.Write("Log       ");
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.Write("Warning   ");
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.Write("Error     ");
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Exception:
                    Console.Write("Exception ");
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Critical  ");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.Write(' ');

            Console.BackgroundColor = ConsoleColor.Black;

            Console.Write(' ');

            Console.WriteLine(message!.ToString());
        }
        catch
        {

        }

        LogLock.Release();
    }
}