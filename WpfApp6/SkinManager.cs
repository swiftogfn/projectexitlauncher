using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using static WpfApp5.MainWindow;

public static class SkinManager
{
    private static readonly string skinDataFile = "lastSkins.json";

    public static List<Skin> LastSkins { get; private set; } = new List<Skin>();

    public static void Load()
    {
        if (File.Exists(skinDataFile))
        {
            string json = File.ReadAllText(skinDataFile);
            LastSkins = JsonSerializer.Deserialize<List<Skin>>(json) ?? new List<Skin>();
        }
    }

    public static void Save()
    {
        string json = JsonSerializer.Serialize(LastSkins, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(skinDataFile, json);
    }

    public static void AddSkin(Skin newSkin)
    {
        // Optional: Remove duplicates by name or id
        LastSkins.RemoveAll(s => s.SkinName == newSkin.SkinName);
        LastSkins.Insert(0, newSkin); // Add newest first

        // Limit list size to e.g. 5 skins
        if (LastSkins.Count > 5)
            LastSkins.RemoveAt(LastSkins.Count - 1);

        Save();
    }
}
