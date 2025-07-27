using System.Net;
using System.Text;
using TNet.Server;

namespace TNet;

internal class Program
{
    static async Task Main(string[] args)
    {
        // basic console initing
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        TaskScheduler.UnobservedTaskException += OnTaskException;

        Console.WriteLine("TNet Backend, made by overmet15.");

        InitEncryption(args);

        _ = Debug.StartFileWriting();

#if DEBUG
        await Lobby.Run(IPAddress.Any, InitPort(args), Game.dinoHunter);
#else
        await Lobby.Run(IPAddress.Any, InitPort(args), Game.dinoHunter); // make game a argument on app launch
#endif
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

    static int InitPort(string[] args)
    {
#if DEBUG
        return 6750;
#else
        int parse;

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out parse)) return parse;
        }

        Console.WriteLine("Input port...");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out parse)) break;
            else Console.WriteLine("Try again...");
        }

        return parse;
#endif
    }

    static void InitEncryption(string[] args)
    {
#if DEBUG
        //Lobby.blowFish = new("Triniti_Tlck");
#else
        if (args.Length > 1)
        {
            if (!string.IsNullOrWhiteSpace(args[1]))
            {
                Lobby.blowFish = new(args[1]);
                return;
            }
        }

        Console.WriteLine("Input the encryption key...");
        string? key = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(key)) Console.WriteLine("No key provided, no encryption will be used.");
        else Lobby.blowFish = new(key);
#endif
    }
}
