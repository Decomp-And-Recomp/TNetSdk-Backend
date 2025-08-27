using System.Net;
using System.Text;
using TNet.Server;

namespace TNet;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        if (!Init(args, out int port, out int game, out bool saveLogs))
        {
            Debug.Write("Press any key to close..");
            Console.ReadKey(true);
            return;
        }

        Game gameParse = (Game)game;

        Console.WriteLine("TNet Backend, made by overmet15.");

        Debug.Write("\n");

        Debug.Write($"Port set to {port}");
        Debug.Write($"Game set to {gameParse}");

        TaskScheduler.UnobservedTaskException += OnTaskException;

        if (saveLogs) _ = Debug.StartFileWriting();

        await BanList.Initialize();

        await Lobby.Run(IPAddress.Any, port, gameParse);
    }

    static bool Init(string[] args, out int port, out int game, out bool saveLogs)
    {
        bool gameSet = false;
        bool portSet = false;
        game = 0;
        port = 0;
        saveLogs = false;

        if (args.Length == 0)
        {
            Debug.Write("Usage: TNet -g {gameId} -p {port}");
            LogGames();
            return false;
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-p")
            {
                if (int.TryParse(args[i+1], out port)) portSet = true;
                else
                {
                    Debug.Write("Port (-p) cannot be parsed as int, make sure its only numbers.", ConsoleColor.Red);
                    return false;
                }
            }
            if (args[i] == "-g")
            {
                if (int.TryParse(args[i + 1], out game)) gameSet = true;
                else
                {
                    Debug.Write("Game (-g) cannot be parsed as int, make sure its only numbers.", ConsoleColor.Red);
                    LogGames();
                    return false;
                }

                if (game < 0 || game >= Enum.GetValues(typeof(Game)).Length)
                {
                    Debug.Write($"Unsupported game id:{game}");
                    LogGames();
                    return false;
                }
            }
            if (args[i] == "-sl") saveLogs = true;
        }

        if (!gameSet)
        { 
            Debug.Write("Game (-g {gameId}) is not set. Defaulting to Default", ConsoleColor.Magenta);
            game = 0;
        }
        if (!portSet)
        {
            Debug.Write("Port (-p {port}) is not set. Defaulting to 6750", ConsoleColor.Magenta);
            port = 6750;
        }

        return true;
    }

    static void LogGames()
    {
        Debug.Write("Aviable games ids:");
        foreach (Game g in Enum.GetValues(typeof(Game)))
        {
            Debug.Write($"{g}: {(int)g}");
        }
    }

    static void OnTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Debug.Log("UNOBSERVED EXCEPTION(S):", ConsoleColor.DarkRed);
        foreach (Exception ex in args.Exception.InnerExceptions)
        {
            Debug.LogException(ex);
        }

        args.SetObserved();
    }
}
