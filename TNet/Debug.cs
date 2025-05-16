namespace TNet
{
    public static class Debug
    {
        static readonly object logLock = new();

        ///<summary>Logs message without any additional text.</summary>
        public static void Log(object message, ConsoleColor color = ConsoleColor.White)
        {
            lock (logLock)
            {
#nullable disable // fucking warning
                string msg = message == null ? string.Empty : message.ToString();
#nullable enable

                if (string.IsNullOrEmpty(msg))
                {
                    LogWarning("The message was empty.");
                    return;
                }

                Console.ForegroundColor = color;
                var v = DateTime.UtcNow;
                Console.WriteLine(v.ToString("[HH:mm:ss:fff] ") + msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void LogInfo(object msg) => Log(msg, ConsoleColor.Cyan);

        ///<summary>Logs message and stack trace</summary>
        public static void LogWarning(object msg)
        {
            Log(msg, ConsoleColor.Yellow);
            LogStackFull(ConsoleColor.DarkYellow);
        }

        ///<summary>Logs message and stack trace</summary>
        public static void LogError(object msg)
        {
            Log(msg, ConsoleColor.Red);
            LogStackFull(ConsoleColor.DarkRed);
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
        public static void LogStackSingle(int frame = 1, ConsoleColor color = ConsoleColor.White)
        {
            System.Diagnostics.StackTrace stackTrace = new(true);

            System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(frame);

            string methodName = callerFrame.GetMethod().Name;
            string className = callerFrame.GetMethod().DeclaringType.FullName;
            int line = callerFrame.GetFileLineNumber();

            Log($"{className}.{methodName}:{line}", color);
        }

        public static void LogStackFull(ConsoleColor color = ConsoleColor.White)
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
}
