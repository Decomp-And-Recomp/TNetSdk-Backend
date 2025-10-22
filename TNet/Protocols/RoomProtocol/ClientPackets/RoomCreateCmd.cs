using TNet.Protocols.Objects;

namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomCreateCmd : IClientPacket
{
    public string roomName = string.Empty;
    public string password = string.Empty;
    public ushort groupId;
    public ushort maxClients;
    public RoomType roomType;
    public RoomSwitchMasterType switchMasterType;
    public string param = string.Empty;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopString(ref roomName)) return false;
        if (!unPacker.PopString(ref password)) return false;
        if (!unPacker.PopUInt16(ref groupId)) return false;
        if (!unPacker.PopUInt16(ref maxClients)) return false;

        ushort rType = 0;
        ushort sType = 0;
        if (!unPacker.PopUInt16(ref rType)) return false;
        if (!unPacker.PopUInt16(ref sType)) return false;

        roomType = (RoomType)rType;
        switchMasterType = (RoomSwitchMasterType)sType;

        return true;
    }
}
