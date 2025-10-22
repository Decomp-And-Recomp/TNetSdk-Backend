using TNet;
using TNet.Helpers;

Console.CursorVisible = false;
Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

#if DEBUG
Variables.encryptionKey = "ExampleKey";
#endif

ushort port = 6750;

for (int i = 0; i < args.Length; i++)
{
    string text;
    ushort result;
    float result2;

    switch (args[i])
    {
        case "-gen":
            text = args[i + 1];
            if (ushort.TryParse(text, out result) && result < 4 && result > 0) Variables.version = (TNet.Version)result;
            else Logger.Error($"'{text}' is not a valid value for '-gen', the bounds are from 1 to 3.");
            break;
        case "-key":
            Variables.encryptionKey = args[i + 1];
            break;
        case "-port":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) port = result;
            else Logger.Error($"'{text}' is not a valid value for '-port', the bounds are from 0 to 65535.");
            break;
        case "-maxClients":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.maxClients = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxClients', the bounds are from 0 to 65535.");
            break;
        case "-maxRooms":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.maxRooms = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxRooms', the bounds are from 0 to 65535.");
            break;
        case "-minRoomVal":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.minRoomValue = result;
            else Logger.Error($"'{text}' is not a valid value for '-minRoomVal', the bounds are from 0 to 65535.");
            break;
        case "-maxRoomVal":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.maxRoomValue = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxRoomVal', the bounds are from 0 to 65535.");
            break;
        case "-heartTime":
            text = args[i + 1];
            if (float.TryParse(text, System.Globalization.NumberStyles.Any, null, out result2)) Variables.heartbeatTimeout = result2;
            else Logger.Error($"'{text}' is not a valid value for '-heartTime', the valid value must be a float (ex: '1.5').");
            break;
    }

    i++;
}

// log

Logger.Info($"Gen set to '{Variables.version}'");

EncrpytionHelper.Initialize();

Logger.Info($"Heartbeat Timeout (seconds): '{Variables.heartbeatTimeout}'");

Logger.Info($"Max rooms: '{Variables.maxRooms}'");
Logger.Info($"Max clients: '{Variables.maxClients}'");

Logger.Info($"Minimum room id: '{Variables.minRoomValue}'");
Logger.Info($"Maximum room id: '{Variables.maxRoomValue}'");

if (Variables.minRoomValue > Variables.maxRoomValue)
{
    Logger.Error("Minimum room id cannot be bigger than maximum. Aborting.");
    return;
}

await Lobby.Run(port);