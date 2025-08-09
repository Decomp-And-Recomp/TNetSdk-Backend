using System.Text;

namespace TNet;

internal static class AdminPanel
{
    public static bool isTyping { get; private set; }

    static bool isRunning = false;

    static readonly StringBuilder helpBuilder = new();

    static readonly Dictionary<string, Action<string[]>> onCommand = [];

    static AdminPanel()
    {
        RegisterCommand("?", (args) => Debug.Log(helpBuilder), "Log this.");
        AdminCommands.SetUp();
    }

    public static void Run()
    {
        if (isRunning)
        {
            Debug.LogWarning("Running twise?");
            return;
        }

        isRunning = true;
        Debug.Log("Press / to input command, input ? for help.");

        try
        {
            InnerConsoleLogic();
        }
        catch (Exception ex)
        {
            Debug.LogException("Exception in AdminPanel", ex);
            isTyping = false;
            isRunning = false;
            Debug.Free();
            Run();
        }
    }

    static void InnerConsoleLogic()
    {
        while (true)
        {
            if (Console.ReadKey(true).KeyChar == '/')
            {
                isTyping = true;

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Waiting for command...");

                string? line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    isTyping = false;
                    continue;
                }

                string[] parammsRaw = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parammsRaw.Length == 0)
                {
                    isTyping = false;
                    continue;
                }

                string command = parammsRaw[0];
                string[] paramms = new string[parammsRaw.Length - 1];
                Array.Copy(parammsRaw, 1, paramms, 0, paramms.Length);

                try
                {
                    RunCommand(command, paramms);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                isTyping = false;

                Debug.Free();
            }
        }
    }

    /// <summary>
    /// Use this for running commands manually trough code. (what retard would do that?)
    /// </summary>
    public static void RunCommand(string command, params string[] args)
    {
        if (onCommand.TryGetValue(command, out Action<string[]>? value)) value.Invoke(args);
        else
        {
            Debug.Log($"Command '{command}' does not exist.");
        }
    }

    public static void RegisterCommand(string command, Action<string[]> action, string? desc = null)
    {
        if (command.Contains(' '))
        {
            Debug.LogError($"A command CANNOT contain spaces in it. ('{command}')");
            return;
        }

        if (onCommand.ContainsKey(command))
        {
            Debug.LogError($"Same command is already registred! ('{command}')");
            return;
        }

        onCommand.Add(command, action);

        if (string.IsNullOrWhiteSpace(desc)) return;

        helpBuilder.AppendLine($"{command}: {desc}");
    }
}
