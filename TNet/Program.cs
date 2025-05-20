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

        Console.WriteLine("TNet Backend, made by overmet15.");

        InitEncryption(args);

        await Lobby.Run(IPAddress.Any, InitPort(args));
    }

    static int InitPort(string[] args)
    {
        int parse;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != "-p") continue;
            
            if (i + 1 < args.Length)
                if (int.TryParse(args[i + 1], out parse)) return parse;

            break;
        }

        Console.WriteLine("Input port...");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out parse)) break;
            else Console.WriteLine("Try again...");
        }

        return parse;
    }

    static void InitEncryption(string[] args)
    {
#if DEBUG
        Lobby.blowFish = new("Triniti_Tlck");
#else
        // ToDo?: make it use "Triniti_Tlck" in DEBUG mode.

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] != "-k") continue;

            if (i + 1 < args.Length) Lobby.blowFish = new(args[i+1]);

            return;
        }

        Console.WriteLine("Input the encryption key...");
        string? key = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(key)) Lobby.blowFish = new(key);
        else Console.WriteLine("No key provided, no encryption will be used.");
#endif
    }
}
