using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TNet.Binary;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TNet.Helpers;

namespace TNet.Protocols;

internal class Packer : BufferWriter
{
    const int minCompressSize = 256;

    public byte[] MakePacket(SystemProtocol.Cmd cmd)
        => MakePacket(1, (ushort)cmd);

    public byte[] MakePacket(RoomProtocol.Cmd cmd)
        => MakePacket(2, (ushort)cmd);

    public byte[] MakePacket(ushort protocol, ushort cmd)
    {
        byte[] data = [.. base.data];

        ushort compressType = 0;

        if (data.Length >= minCompressSize)
        {
            MemoryStream memoryStream = new();
            DeflaterOutputStream deflaterOutputStream = new(memoryStream);
            deflaterOutputStream.Write(data, 0, data.Length);
            deflaterOutputStream.Close();
            data = memoryStream.ToArray();
            compressType = 1;
        }

        int packetLength = Lobby.headerSize +  data.Length;

        BufferWriter w = new();
        w.PushUInt16((ushort)packetLength);
        w.PushUInt16(1);
        w.PushUInt16(protocol);
        w.PushUInt16(cmd);
        w.PushUInt16(compressType);
        w.PushByteArray(data);

        byte[] finalArray = w.ToByteArray();
        EncrpytionHelper.Encrypt(finalArray);

        return finalArray;
    }
}
