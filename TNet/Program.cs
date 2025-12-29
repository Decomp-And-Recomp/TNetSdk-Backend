using TNet;
using TNet.Helpers;

Console.CursorVisible = false;
Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

#if DEBUG
Variables.EncryptionKey = "Triniti_Tlck";
#endif

ushort port = 6750;

for (int i = 0; i < args.Length; i++)
{
    string text;
    ushort result;
#pragma warning disable IDE0018
    float result2;
#pragma warning restore
    switch (args[i])
    {
        case "-gen":
            text = args[i + 1];
            if (ushort.TryParse(text, out result) && result < 4 && result > 0) Variables.Version = (TNet.Version)result;
            else Logger.Error($"'{text}' is not a valid value for '-gen', the bounds are from 1 to 3.");
            break;
        case "-key":
            Variables.EncryptionKey = args[i + 1];
            break;
        case "-port":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) port = result;
            else Logger.Error($"'{text}' is not a valid value for '-port', the bounds are from 0 to 65535.");
            break;
        case "-maxClients":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.MaxClients = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxClients', the bounds are from 0 to 65535.");
            break;
        case "-maxRooms":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.MaxRooms = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxRooms', the bounds are from 0 to 65535.");
            break;
        case "-minRoomVal":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.MinRoomValue = result;
            else Logger.Error($"'{text}' is not a valid value for '-minRoomVal', the bounds are from 0 to 65535.");
            break;
        case "-maxRoomVal":
            text = args[i + 1];
            if (ushort.TryParse(text, out result)) Variables.MaxRoomValue = result;
            else Logger.Error($"'{text}' is not a valid value for '-maxRoomVal', the bounds are from 0 to 65535.");
            break;
        case "-heartTime":
            text = args[i + 1];
            if (float.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out result2)) Variables.HeartbeatTimeout = result2;
            else Logger.Error($"'{text}' is not a valid value for '-heartTime', the valid value must be a float (ex: '1.5').");
            break;
    }

    i++;
}

// log

Logger.Info($"Gen set to '{Variables.Version}'");

EncrpytionHelper.Initialize();

Logger.Info($"Heartbeat Timeout (seconds): '{Variables.HeartbeatTimeout}'");

Logger.Info($"Max rooms: '{Variables.MaxRooms}'");
Logger.Info($"Max clients: '{Variables.MaxClients}'");

Logger.Info($"Minimum room id: '{Variables.MinRoomValue}'");
Logger.Info($"Maximum room id: '{Variables.MaxRoomValue}'");

if (Variables.MinRoomValue > Variables.MaxRoomValue)
{
    Logger.Error("Minimum room id cannot be bigger than maximum. Aborting.");
    return;
}

// Prevent from typing.
_ = Task.Run(() => {
    while (true) Console.ReadKey(true);
    });

await Lobby.Run(port);