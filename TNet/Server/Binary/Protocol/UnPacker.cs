using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TNet.Server.Binary.Protocol;

internal class UnPacker : BufferReader
{
    readonly Header header = new();

    public virtual bool ParserPacket(Packet packet)
    {
        SetData(packet.ByteArray());
        if (!PopUInt16(ref header.m_sLength))
        {
            return false;
        }
        if (!PopUInt16(ref header.m_sVersion))
        {
            return false;
        }
        if (!PopUInt16(ref header.m_sProtocol))
        {
            return false;
        }
        if (!PopUInt16(ref header.m_sCmd))
        {
            return false;
        }
        if (!PopUInt16(ref header.m_sCompressType))
        {
            return false;
        }
        if (header.m_sCompressType == 1)
        {
            InflaterInputStream inflaterInputStream = new(new MemoryStream(m_data, m_offset, m_data.Length - m_offset));
            MemoryStream memoryStream = new();
            int num;
            byte[] array = new byte[4096];
            while ((num = inflaterInputStream.Read(array, 0, array.Length)) != 0)
            {
                memoryStream.Write(array, 0, num);
            }
            SetData(memoryStream.ToArray());
        }
        return true;
    }

    public ushort GetLength()
    {
        return header.m_sLength;
    }

    public ushort GetProtocol()
    {
        return header.m_sProtocol;
    }

    public ushort GetCmd()
    {
        return header.m_sCmd;
    }
}
