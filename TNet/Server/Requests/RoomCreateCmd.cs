using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Requests;

internal class RoomCreateCmd
{
    public string roomName = string.Empty;
    public string password = string.Empty;
    public string param = string.Empty;

    public ushort groupId;
    public ushort maxUsers;

    public RoomType roomType;
    public RoomSwitchMasterType roomSwitchMasterType;

    RoomCreateCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomCreateCmd cmd)
    {
        ushort tempUshort = 0;

        cmd = new();

        if (!unPacker.PopString(ref cmd.roomName, Encoding.ASCII)) return false;
        if (!unPacker.PopString(ref cmd.password, Encoding.ASCII)) return false;

        if (!unPacker.PopUInt16(ref cmd.groupId)) return false;
        if (!unPacker.PopUInt16(ref cmd.maxUsers)) return false;

        if (!unPacker.PopUInt16(ref tempUshort)) return false;
        cmd.roomType = (RoomType)tempUshort;

        if (!unPacker.PopUInt16(ref tempUshort)) return false;
        cmd.roomSwitchMasterType = (RoomSwitchMasterType)tempUshort;

        if (!unPacker.PopString(ref cmd.password, Encoding.ASCII)) return false;

        return true;
    }
}
