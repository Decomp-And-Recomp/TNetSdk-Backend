namespace TNet.Server;

internal static class BanList
{
    static string? basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    static string path = "banlist.txt";

    static List<string> list = [];

    public static async Task Initialize()
    {
        Debug.LogInfo("Initializing Ban list");

        if (basePath == null) throw new ArgumentNullException(nameof(basePath));

        path = Path.Combine(basePath, path);

        if (!File.Exists(path))
        {
            File.Create(path);
            return;
        }

        string text = await File.ReadAllTextAsync(path);

        foreach (string s in text.Split("\n"))
        {
            if (string.IsNullOrWhiteSpace(s) || s.TrimStart().StartsWith('#')) continue;

            list.Add(s.Trim());
        }
    }

    public static bool IsBanned(string ip)
    {
        return list.Contains(ip);
    }
}
