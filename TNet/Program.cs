using System.Net;
using System.Text;
using TNet.Server;

namespace TNet;

internal class Program
{
#pragma warning disable IDE0060
    static async Task Main(string[] args)
#pragma warning restore
    {
        // basic console initing
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("TNet Backend, made by overmet15.");

        Console.WriteLine("Please write the port to host on...");

        int port;

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out port)) break;

            Console.WriteLine("Try again...");
        }

        _ = Task.Run(AdminPanel.Run);
        await Lobby.Run(IPAddress.Any, port);
    }
}
