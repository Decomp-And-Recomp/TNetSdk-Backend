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

        if (!Init(args, out int port, out int game)) return;
        Game gameParse = (Game)game;

        Console.WriteLine("TNet Backend, made by overmet15.");

        Debug.Write("\n");

        Debug.Write($"Port set to {port}");
        Debug.Write($"Game set to {gameParse}");

        TaskScheduler.UnobservedTaskException += OnTaskException;

        _ = Debug.StartFileWriting();

        await Lobby.Run(IPAddress.Any, port, gameParse);
    }

    static bool Init(string[] args, out int port, out int game)
    {
        bool gameSet = false;
        bool portSet = false;
        game = 0;
        port = 0;

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
                    Debug.Write("Aviable games: ");
                    foreach (Game g in Enum.GetValues(typeof(Game)))
                    {
                        Debug.Write($"{g}: {(int)g}");
                    }
                    return false;
                }
            }
        }

        if (!gameSet && !portSet)
        {
            Debug.Write("Game and Port (-g {gameId} -p {port}) were not set. Make sure that atleast the game is set", ConsoleColor.Red);
            return false;
        }
        else if (!gameSet)
        { 
            Debug.Write("Game (-g {gameId}) is not set. Make sure that the game is set", ConsoleColor.Red);
        }
        else if (!portSet)
        {
            Debug.Write("Port (-p {port}) is not set. Defaulting to 6750");
            port = 6750;
        }

        return true;
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
