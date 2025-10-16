using System.Text;

namespace TNet.Binary;

#pragma warning disable

internal class BufferWriter
{
    public List<byte> data = [];

    public void PushByte(byte b)
    {
        data.Add(b);
    }

    public void PushUInt16(ushort Num)
    {
        byte item = (byte)((uint)((Num & 0xFF00) >> 8) & 0xFFu);
        byte item2 = (byte)(Num & 0xFFu & 0xFFu);
        data.Add(item);
        data.Add(item2);
    }

    public void PushUInt32(uint Num)
    {
        byte item = (byte)(((Num & 0xFF000000u) >> 24) & 0xFFu);
        byte item2 = (byte)(((Num & 0xFF0000) >> 16) & 0xFFu);
        byte item3 = (byte)(((Num & 0xFF00) >> 8) & 0xFFu);
        byte item4 = (byte)(Num & 0xFFu & 0xFFu);
        data.Add(item);
        data.Add(item2);
        data.Add(item3);
        data.Add(item4);
    }

    public void PushUInt64(ulong Num)
    {
        uint num = (uint)(Num >> 32);
        uint num2 = (uint)Num;
        PushUInt32(num);
        PushUInt32(num2);
    }

    public void PushByteArray(byte[] buf)
        => data.AddRange(buf);

    public void PushByteArray(byte[] buf, int length)
    {
        for (int i = 0; i < length; i++)
        {
            data.Add(buf[i]);
        }
    }

    public void PushByteList(List<byte> list)
        => data.AddRange(list);

    public byte[] ToByteArray()
    {
        return [.. data];
    }

    public void PushString(string str)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(str);
        PushUInt16((ushort)bytes.Length);

        data.AddRange(bytes);
    }

    public void PushString(string str, int padding, bool nullByteAtEnd = true)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(str);
        byte[] post = new byte[padding];

        Array.Copy(bytes, post, bytes.Length > 16 ? 16 : bytes.Length);

        if (nullByteAtEnd) post[padding - 1] = 0;

        data.AddRange(post);
    }

    public void PushInt64(long Num)
        => PushUInt64(unchecked((ulong)Num));
}