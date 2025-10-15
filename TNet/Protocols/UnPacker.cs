using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using TNet.Binary;

namespace TNet.Protocols;

internal class UnPacker : BufferReader
{
    ushort _length;
    public ushort length => _length;

    ushort _version;
    public ushort version => _version;

    Protocol _protocol;
    public Protocol protocol => _protocol;

    ushort _cmd;
    public ushort cmd => _cmd;

    public bool Initialize()
    {
        if (!PopUInt16(ref _length)) return false;
        if (!PopUInt16(ref _version)) return false;

        ushort protocol = 0;
        if (!PopUInt16(ref protocol)) return false;
        if (!PopUInt16(ref _cmd)) return false;

        ushort compressionType = 0;
        if (!PopUInt16(ref compressionType)) return false;

        _protocol = (Protocol)protocol;

        if (compressionType == 1)
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
}
