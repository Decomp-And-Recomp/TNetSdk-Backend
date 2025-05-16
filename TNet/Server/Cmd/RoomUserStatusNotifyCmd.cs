using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomUserStatusNotifyCmd : UnPacker
{
	public ushort m_user_id;

	public ushort m_key;

	//public SFSObject sfs_object;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!PopUInt16(ref m_user_id))
		{
			return false;
		}
		if (!PopUInt16(ref m_key))
		{
			return false;
		}
		ushort val = 0;
		if (!PopUInt16(ref val))
		{
			return false;
		}
		byte[] val2 = new byte[val];
		if (!PopByteArray(ref val2, val))
		{
			return false;
		}
		//ByteArray ba = new ByteArray(val2);
		//sfs_object = SFSObject.NewFromBinaryData(ba);
		return true;
	}
}
