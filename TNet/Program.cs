using TNet;
using TNet.Helpers;

Console.CursorVisible = false;
Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

#if DEBUG
Variables.EncryptionKey = "Triniti_Tlck";
#endif

ushort port = 6750;

bool TryGetArg(string entry, out string arg)
{
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == entry && i + 1 < args.Length)
        {
            arg = args[i + 1];
            return true;
        }
    }

    arg = string.Empty;

    return false;
}

bool TryGetInt(string entry, out int arg)
{
    arg = 0;

    if (TryGetArg(entry, out var v))
    {
        if (int.TryParse(v, out arg)) return true;
        else Logger.Error($"Argument '{entry}' has to be a integer.");
    }

    return false;
}

if (TryGetInt("-gen", out var gen))
{
    if (Math.Clamp(gen, 0, 3) == gen) Variables.Version = (TNet.Version)gen;
    else Logger.Error($"'{gen}' is not a valid value for '-gen', the bounds are from 0 to 2.");
}

if (TryGetArg("-key", out var key)) Variables.EncryptionKey = key;

if (TryGetInt("-port", out var portInt))
{
    if (Math.Clamp(portInt, 0, ushort.MaxValue) == portInt) port = (ushort)portInt;
    else Logger.Error($"'{port}' is not a valid value for '-port', the bounds are from 0 to 65535.");
}

if (TryGetInt("-maxClients", out var maxClients))
{
    if (Math.Clamp(maxClients, 0, ushort.MaxValue) == maxClients) Variables.MaxClients = (ushort)maxClients;
    else Logger.Error($"'{maxClients}' is not a valid value for '-maxClients', the bounds are from 0 to 65535.");
}

if (TryGetInt("-maxRooms", out var maxRooms))
{
    if (Math.Clamp(maxRooms, 0, ushort.MaxValue) == maxClients) Variables.MaxRooms = (ushort)maxRooms;
    else Logger.Error($"'{maxRooms}' is not a valid value for '-maxRooms', the bounds are from 0 to 65535.");
}

if (TryGetArg("-heartTime", out var heartTimeText))
{
    if (float.TryParse(heartTimeText, System.Globalization.CultureInfo.InvariantCulture, out var heartTime))
        Variables.HeartbeatTimeout = heartTime;
    else Logger.Error($"'{heartTimeText}' is not a valid value for '-heartTime', the valid value must be a float (ex: '1.5').");
}

/*for (int i = 0; i < args.Length; i++)
{
    string text;
    ushort result;
#pragma warning disable IDE0018
    float result2;
#pragma warning restore
    switch (args[i])
    {
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
    }

    i++;
}*/

// log

Logger.Info($"Gen set to '{Variables.Version}'");

EncrpytionHelper.Initialize();

Logger.Info($"Heartbeat Timeout (seconds): '{Variables.HeartbeatTimeout}'");

Logger.Info($"Max rooms: '{Variables.MaxRooms}'");
Logger.Info($"Max clients: '{Variables.MaxClients}'");

//Logger.Info($"Minimum room id: '{Variables.MinRoomValue}'");
//Logger.Info($"Maximum room id: '{Variables.MaxRoomValue}'");

if (Variables.MinRoomValue > Variables.MaxRoomValue)
{
    Logger.Error("Minimum room id cannot be bigger than maximum. Aborting.");
    return;
}

// Prevent from typing.
_ = Task.Run(() => { while (true) Console.ReadKey(true); });

await Lobby.Run(port);