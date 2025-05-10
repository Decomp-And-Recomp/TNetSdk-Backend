namespace TNet.Server.Binary;

internal static class BinaryUtils
{
    public static string PopString(BufferReader reader, System.Text.Encoding encoding)
    {
        ushort length = 0;

        reader.PopUInt16(ref length);

        byte[] buffer = new byte[length];
        reader.PopByteArray(ref buffer, length);

        return encoding.GetString(buffer);
    }

    public static void PushString(BufferWriter writer, string value, System.Text.Encoding encoding)
    {
        ushort length = 0;
        byte[] bytes = encoding.GetBytes(value);

        writer.PushUInt16(length);
        writer.PushByteArray(bytes, bytes.Length);
    }
}
