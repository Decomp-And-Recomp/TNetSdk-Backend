using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomVarNotifyCmd
{
    /// <summary>Notifies people in the room. that some person changed the room var.</summary>
    /// <param name="userId">Id of the client that changed the variable.</param>
    /// <param name="key">Key of the variable.</param>
    /// <param name="data">The variable, in bytes.</param>
    /// <returns>A packet that has to be sent to EVERY client in the room.</returns>
    public static Packet Notify(ushort userId, ushort key, byte[] data)
    {
        Packer packer = new();
        packer.PushUInt16(userId);
        packer.PushUInt16(key);

        packer.PushUInt16((ushort)data.Length);
        packer.PushByteArray(data, data.Length);

        return packer.MakePacket(RoomCMD.var_notify);
    }
}
