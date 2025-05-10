using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class SysLoginCmd
{
	public readonly string account, password, nickname;

	/*public SysLoginCmd(string account, string pwd, string nickname)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(account);
		PushUInt16((ushort)bytes.Length);
		PushByteArray(bytes, bytes.Length);
		byte[] bytes2 = Encoding.ASCII.GetBytes(pwd);
		PushUInt16((ushort)bytes2.Length);
		PushByteArray(bytes2, bytes2.Length);
		byte[] bytes3 = Encoding.ASCII.GetBytes(nickname);
		PushUInt16((ushort)bytes3.Length);
		PushByteArray(bytes3, bytes3.Length);
	}*/

	public SysLoginCmd(UnPacker unPacker)
	{
		account = BinaryUtils.PopString(unPacker, Encoding.ASCII);
        password = BinaryUtils.PopString(unPacker, Encoding.ASCII);
        nickname = BinaryUtils.PopString(unPacker, Encoding.ASCII);
    }

}
