namespace TNet;

public static class Debug
{
    readonly struct Item(object? message, ConsoleColor color)
    {
        public readonly object? message = message;
        public readonly ConsoleColor color = color;
    }

    static readonly object logLock = new();

    static readonly List<Item> items = [];

    ///<summary>Logs message without any additional text.</summary>
    public static void Log(object? message, ConsoleColor color = ConsoleColor.White)
    {
        lock (logLock)
        {
            string msg = message?.ToString() ?? string.Empty;

            var time = DateTime.UtcNow;

            if (string.IsNullOrEmpty(msg))
            {
                LogWarning("The message was empty.");
                return;
            }

            if (AdminPanel.isTyping)
            {
                items.Add(new(message, color));
                return;
            }

            Console.ForegroundColor = color;
            Console.WriteLine(time.ToString("[HH:mm:ss:fff] ") + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    /// <summary>
    /// Frees all the items that were hold due to AdminPanel being active.
    /// (if <see cref="AdminPanel.isTyping"/> is false)
    /// </summary>
    public static void Free()
    {
        lock (logLock)
        {
            if (AdminPanel.isTyping) return;

            for (int i = 0; i < items.Count; i++)
            {
                Console.ForegroundColor = items[i].color;
                Console.WriteLine(items[i].message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            items.Clear();
        }
    }

    public static void LogInfo(object msg) => Log(msg, ConsoleColor.Cyan);

    ///<summary>Logs message and stack trace</summary>
    public static void LogWarning(object msg)
    {
        Log(msg, ConsoleColor.Yellow);
        LogStack(ConsoleColor.DarkYellow);
    }

    ///<summary>Logs message and stack trace</summary>
    public static void LogError(object msg)
    {
        Log(msg, ConsoleColor.Red);
        LogStack(ConsoleColor.DarkRed);
    }

    ///<summary>Logs [message] (from exception) and [stack trace]</summary>
    public static void LogException(Exception ex, bool includeStackTrace = true)
    {
        Log($"[message]: {ex.Message}", ConsoleColor.Red);
        if (includeStackTrace) Log($"[stack trace]: {ex.StackTrace}", ConsoleColor.Red);
    }

    ///<summary>Logs message with [message] (from exception) and [stack trace]</summary>
    public static void LogException(object message, Exception ex, bool includeStackTrace = true)
    {
        Log(message, ConsoleColor.Red);
        Log($"[message]: {ex.Message}", ConsoleColor.Red);
        if (includeStackTrace) Log($"[stack trace]: {ex.StackTrace}", ConsoleColor.Red);
    }

#pragma warning disable
    public static void LogStack(ConsoleColor color = ConsoleColor.White)
    {
        System.Diagnostics.StackTrace stackTrace = new(true);

        for (int i = 1; i < stackTrace.FrameCount; i++) // yes i is supposed to start with 1
        {
            System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(i);

            string methodName = callerFrame.GetMethod().Name;
            string className = callerFrame.GetMethod().DeclaringType.FullName;
            int line = callerFrame.GetFileLineNumber();

            Log($"{className}.{methodName}:{line}", color);
        }
    }
#pragma warning restore

    public static void Pause()
    {
        Log("Press any key to continue..", ConsoleColor.DarkCyan);
        Console.ReadKey(false);
    }
}
