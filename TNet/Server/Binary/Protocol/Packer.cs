using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TNet.Exceptions;

namespace TNet.Server.Binary.Protocol;

internal class Packer : BufferWriter
{
    const int COMPRESS_SIZE = 256;

    public Packet MakePacket(SysCMD cmd, bool allow_compress = true)
        => MakePacket(1, (ushort)cmd, allow_compress);

    public Packet MakePacket(RoomCMD cmd, bool allow_compress = true)
        => MakePacket(2, (ushort)cmd, allow_compress);

    public Packet MakePacket(ushort protocol, ushort cmd, bool allow_compress = true)
    {
        byte[] data = [.. m_data];

        ushort sCompressType = 0;
        if (allow_compress && data.Length >= COMPRESS_SIZE)
        {
            MemoryStream memoryStream = new();
            DeflaterOutputStream deflaterOutputStream = new(memoryStream);
            deflaterOutputStream.Write(data, 0, data.Length);
            deflaterOutputStream.Close();
            data = memoryStream.ToArray();
            sCompressType = 1;
        }

        int packetLength = Header.HEADER_LENGTH + data.Length;

        // youre not supposed to have THAT big packet,
        // but cutting off the data will cause issues on client side
        if (!Packet.LengthIsVaild(packetLength)) 
            throw new InvalidPacketLengthException();

        Header header = new()
        {
            m_sLength = (ushort)packetLength,
            m_sVersion = 1,
            m_sProtocol = protocol,
            m_sCmd = cmd,
            m_sCompressType = sCompressType
        };

        Packet packet = new(packetLength);

        packet.PushUInt16(header.m_sLength);
        packet.PushUInt16(header.m_sVersion);
        packet.PushUInt16(header.m_sProtocol);
        packet.PushUInt16(header.m_sCmd);
        packet.PushUInt16(header.m_sCompressType);
        packet.PushByteArray(data);

        return packet;
    }
}
