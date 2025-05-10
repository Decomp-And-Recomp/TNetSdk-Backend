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

        await Lobby.Run(IPAddress.Parse("127.0.0.4"), 5000);
    }
}
