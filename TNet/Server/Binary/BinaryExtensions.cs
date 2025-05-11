using System.Text;

namespace TNet.Server.Binary;

internal static class BinaryExtensions
{
    public static void PushString(this BufferWriter writer, string value, System.Text.Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(value);

        writer.PushUInt16((ushort)bytes.Length);
        writer.PushByteArray(bytes, bytes.Length);
    }

    public static bool PopString(this BufferReader reader, ref string str, Encoding encoding)
    {
        ushort length = 0;

        if (!reader.PopUInt16(ref length)) return false;

        byte[] buffer = new byte[length];
        if (!reader.PopByteArray(ref buffer, length)) return false;

        str = encoding.GetString(buffer);

        return true;
    }
}
