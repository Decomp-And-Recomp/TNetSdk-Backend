using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomCreateCmd
{
	public readonly string roomName, password, param;
	public readonly ushort groupId, maxUsers;
    public readonly RoomType roomType;
	public readonly RoomSwitchMasterType roomSwitchMasterType;

	public enum RoomType
	{
		open = 0,
		limit = 1
	}

	public enum RoomSwitchMasterType
	{
		None = 0,
		Auto = 1
	}

	/*public RoomCreateCmd(string room_name, string pwd, ushort group_id, ushort max_user, RoomType room_limit, RoomSwitchMasterType matertype, string parm)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(room_name);
		PushUInt16((ushort)bytes.Length);
		PushByteArray(bytes, bytes.Length);
		byte[] bytes2 = Encoding.ASCII.GetBytes(pwd);
		PushUInt16((ushort)bytes2.Length);
		PushByteArray(bytes2, bytes2.Length);
		PushUInt16(group_id);
		PushUInt16(max_user);
		PushUInt16((ushort)room_limit);
		PushUInt16((ushort)matertype);
		byte[] bytes3 = Encoding.ASCII.GetBytes(parm);
		PushUInt16((ushort)bytes3.Length);
		PushByteArray(bytes3, bytes3.Length);
	}*/

	public RoomCreateCmd(UnPacker unPacker)
    {
        ushort tempUshort = 0;

        // name
        unPacker.PopUInt16(ref tempUshort);

		byte[] nameBytes = new byte[tempUshort];
		unPacker.PopByteArray(ref nameBytes, tempUshort);
		roomName = Encoding.ASCII.GetString(nameBytes);

		// password
		unPacker.PopUInt16(ref tempUshort);

		byte[] passwordBytes = new byte[tempUshort];
		unPacker.PopByteArray(ref passwordBytes, tempUshort);

		password = Encoding.ASCII.GetString(passwordBytes);

		// rest
		unPacker.PopUInt16(ref groupId);
		unPacker.PopUInt16(ref maxUsers);

		// room type
		unPacker.PopUInt16(ref tempUshort);
		this.roomType = (RoomType)tempUshort;

        // room master type
        unPacker.PopUInt16(ref tempUshort);
        this.roomSwitchMasterType = (RoomSwitchMasterType)tempUshort;

		// param
		unPacker.PopUInt16(ref tempUshort);
		byte[] paramBytes = new byte[tempUshort];
		unPacker.PopByteArray(ref paramBytes, tempUshort);
		param = Encoding.ASCII.GetString(paramBytes);
    }
}
