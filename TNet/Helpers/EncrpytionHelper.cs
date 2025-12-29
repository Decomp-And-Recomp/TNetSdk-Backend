using TNet.External;

namespace TNet.Helpers;

internal static class EncrpytionHelper
{
    private static BlowFish? BlowFish;

    public static void Initialize()
    {
        if (string.IsNullOrWhiteSpace(Variables.EncryptionKey)) Logger.Warning("Not using any encryption, key is not set.");
        else
        {
            Logger.Info("Initializing encryption..");
            BlowFish = new(Variables.EncryptionKey);
        }
    }

    public static void Decrypt(List<byte> data)
    {
        if (BlowFish == null) return;

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        BlowFish.Decrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);
    }

    public static void Encrypt(byte[] data)
    {
        if (BlowFish == null) return;

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        BlowFish.Encrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);
    }
}
