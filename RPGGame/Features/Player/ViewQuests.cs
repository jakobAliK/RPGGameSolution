namespace RPGGame;

public static class ViewQuestsHelper
{
    public static void ViewQuests(Player player)
    {
        Console.Clear();

        Console.WriteLine("==========================================");
        Console.WriteLine("                  QUESTS");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        if (player.Quests == null || player.Quests.Count == 0)
        {
            Console.WriteLine("Du hast keine Quests.");
            Console.WriteLine();
            Console.WriteLine("Drücke eine Taste, um zurückzukehren...");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < player.Quests.Count; i++)
        {
            var quest = player.Quests[i];

            string status;

            if (quest.Claimed)
            {
                status = "ABGESCHLOSSEN";
            }
            else if (quest.IsCompleted())
            {
                status = "ABGEBROCHEN - Belohnung verfügbar";
            }
            else
            {
                status = $"{quest.CurrentKills}/{quest.RequiredKills}";
            }

            Console.WriteLine($"{i + 1}. {quest.Name}");
            Console.WriteLine($"   {quest.Description}");
            Console.WriteLine($"   Fortschritt: {status}");
            Console.WriteLine($"   Belohnung: {quest.RewardXP} XP + {quest.RewardGold} Gold");
            Console.WriteLine();
        }

        Console.WriteLine("Drücke eine Taste, um zurückzukehren...");
        Console.ReadKey();
    }
}
