using System.Text.Json;

namespace RPGGame;

// ============================================================
// SAVE SYSTEM
// ============================================================

public static class SaveGame
{
    public static void Save(
        Player player,
        string path = "savegame.json")
    {
        JsonSerializerOptions options =
            new()
            {
                WriteIndented = true
            };

        string json =
            JsonSerializer.Serialize(
                player,
                options);

        File.WriteAllText(
            path,
            json);
    }

    public static Player Load(
        string path = "savegame.json")
    {
        if (!File.Exists(path))
        {
            return null;
        }

        string json =
            File.ReadAllText(path);

        Player player =
            JsonSerializer.Deserialize<Player>(
                json);

        if (player == null)
        {
            return null;
        }

        if (player.Inventory == null)
        {
            player.Inventory = new Inventory();
        }

        if (player.Quests == null)
        {
            player.Quests = new List<Quest>();
        }

        if (player.KillStatistics == null)
        {
            player.KillStatistics =
                new Dictionary<string, int>();
        }

        player.UpdateStats();

        return player;
    }
}
