using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Responses;

internal class RoomLockResCmd : UnPacker
{
    public static Packet Response(bool success, string key = "")
    {
        Packer packer = new();

        packer.PushUInt16(success ? (ushort)0 : (ushort)1);
        packer.PushString(key, Encoding.ASCII);

        return packer.MakePacket(RoomCMD.lock_res);
    }
}
