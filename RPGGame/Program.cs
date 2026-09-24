using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json.Serialization;

namespace RPGGame;


// ============================================================
// QUEST
// ============================================================

public class Quest
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";

    public string TargetMonster { get; private set; } = "";

    public int RequiredKills { get; private set; }
    public int CurrentKills { get; private set; }

    public int RewardXP { get; private set; }
    public int RewardGold { get; private set; }

    public bool Claimed { get; private set; }

    public Quest()
    {
    }
    public Quest(
        string name,
        string description,
        string targetMonster,
        int requiredKills,
        int rewardXP,
        int rewardGold)
    {
        Name = name;
        Description = description;
        TargetMonster = targetMonster;
        RequiredKills = requiredKills;
        RewardXP = rewardXP;
        RewardGold = rewardGold;
    }







    public bool IsCompleted()
    {
        return CurrentKills >= RequiredKills;
    }

    public bool RegisterKill(string monsterName)
    {
        if (Claimed)
        {
            return false;
        }

        if (!monsterName.Equals(
                TargetMonster,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (CurrentKills < RequiredKills)
        {
            CurrentKills++;
            return true;
        }

        return false;
    }

    public bool Claim(Player player)
    {
        if (!IsCompleted() || Claimed)
        {
            return false;
        }

        Claimed = true;

        player.GainExperience(RewardXP);
        player.AddGold(RewardGold);

        return true;
    }
}





// ============================================================
// GAME FACTORY
// ============================================================

public static class GameFactory
{
    // --------------------------------------------------------
    // STARTER PLAYER
    // --------------------------------------------------------

    public static Player CreatePlayer(
        string name)
    {
        Player player =
            new Player(name);

        player.Inventory.Add(
            new Item(
                "Heiltrank",
                "Heilt 30 HP.",
                ItemType.Verbrauchbar,
                20,
                30));

        player.Inventory.Add(
            new Item(
                "Dolch",
                "Ein einfacher Start-Dolch.",
                ItemType.Waffe,
                30,
                3));

        player.Inventory.Add(
            new Item(
                "Lederrüstung",
                "Eine leichte Rüstung.",
                ItemType.Rüstung,
                40,
                2));

        player.Equip(
            player.Inventory.Find("Dolch"));

        player.Equip(
            player.Inventory.Find("Lederrüstung"));

        // Quests
        player.AddQuest(
            new Quest(
                "Goblin-Jäger",
                "Besiege 3 Goblins.",
                "Goblin",
                3,
                100,
                50));

        player.AddQuest(
            new Quest(
                "Wolfjäger",
                "Besiege 2 Wölfe.",
                "Wolf",
                2,
                120,
                70));

        player.AddQuest(
            new Quest(
                "Untote Bedrohung",
                "Besiege 2 Skelette.",
                "Skelett",
                2,
                180,
                100));

        return player;
    }

    // --------------------------------------------------------
    // GOBLIN
    // --------------------------------------------------------

    public static Monster CreateGoblin()
    {
        Monster monster =
            new Monster(
                "Goblin",
                40,
                9,
                2,
                40,
                5,
                15);

        monster.CriticalChance = 5;
        monster.DodgeChance = 8;

    monster.AddDrop(
            new Item(
                "Goblin-Dolch",
                "Ein rostiger Dolch.",
                ItemType.Waffe,
                25,
                2));
        monster.AddDrop(
            new Item(
                "Heiltrank",
                "Heilt 30 HP.",
                ItemType.Verbrauchbar,
                20,
                30));
        monster.AddDrop(
            new Item(
                "Goblin-Dolch",
                "Ein rostiger Dolch.",
                ItemType.Waffe,
                25,
                2));

        return monster;
    }

    // --------------------------------------------------------
    // WOLF
    // --------------------------------------------------------

    public static Monster CreateWolf()
    {
        Monster monster =
            new Monster(
                "Wolf",
                55,
                12,
                3,
                55,
                8,
                18);

        monster.DodgeChance = 15;

        monster.PoisonChance = 10;

        monster.AddDrop(
            new Item(
                "Wolfspelz",
                "Ein wertvoller Pelz.",
                ItemType.Material,
                15));

        monster.AddDrop(
            new Item(
                "Heiltrank",
                "Heilt 30 HP.",
                ItemType.Verbrauchbar,
                20,
                30));

        return monster;
    }

    // --------------------------------------------------------
    // BANDIT
    // --------------------------------------------------------

    public static Monster CreateBandit()
    {
        Monster monster =
            new Monster(
                "Bandit",
                70,
                15,
                5,
                75,
                15,
                35);

        monster.CriticalChance = 15;

        monster.AddDrop(
            new Item(
                "Banditenschwert",
                "Ein gebrauchtes Schwert.",
                ItemType.Waffe,
                50,
                5));

        monster.AddDrop(
            new Item(
                "Goldbeutel",
                "Kann für Gold verkauft werden.",
                ItemType.Material,
                25));

        return monster;
    }

    // --------------------------------------------------------
    // SKELETT
    // --------------------------------------------------------

    public static Monster CreateSkeleton()
    {
        Monster monster =
            new Monster(
                "Skelett",
                80,
                17,
                7,
                100,
                20,
                40);

        monster.AddDrop(
            new Item(
                "Knochenrüstung",
                "Eine ungewöhnliche Rüstung.",
                ItemType.Rüstung,
                60,
                5));

        monster.AddDrop(
            new Item(
                "Großer Heiltrank",
                "Heilt 70 HP.",
                ItemType.Verbrauchbar,
                40,
                70));

        return monster;
    }

    // --------------------------------------------------------
    // BOSS
    // --------------------------------------------------------

    public static Monster CreateDarkKnight()
    {
        Monster monster =
            new Monster(
                "Dunkler Ritter",
                220,
                25,
                10,
                300,
                100,
                180);

        monster.CriticalChance = 20;
        monster.DodgeChance = 10;

        monster.AddDrop(
            new Item(
                "Dunkelschwert",
                "Das Schwert des dunklen Ritters.",
                ItemType.Waffe,
                150,
                12));

        monster.AddDrop(
            new Item(
                "Ritterrüstung",
                "Eine mächtige Rüstung.",
                ItemType.Rüstung,
                140,
                10));

        monster.AddDrop(
            new Item(
                "Großer Heiltrank",
                "Heilt 70 HP.",
                ItemType.Verbrauchbar,
                40,
                70));

        return monster;
    }
}


// ============================================================
// PROGRAM
// ============================================================

public static class Program
{
    public static void Main()
    {
        Console.Title =
            "Das vergessene Königreich";

        Player player =
            StartMenu();

        if (player == null)
        {
            return;
        }

        MainMenu(player);
    }

    // ========================================================
    // STARTMENÜ
    // ========================================================

    private static Player StartMenu()
    {
        Console.Clear();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "      DAS VERGESSENE KÖNIGREICH");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine();

        Console.WriteLine("1. Neues Spiel");
        Console.WriteLine("2. Spiel laden");
        Console.WriteLine("3. Beenden");

        Console.WriteLine();
        Console.Write("Auswahl: ");

        string choice =
            ReadChoiceWithKeywords(
                "Neues Spiel",
                "Spiel laden",
                "Beenden");

        if (choice == "1")
        {
            Console.Clear();

            Console.Write("Name des Helden: ");

            string name =
                Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Held";
            }

            return GameFactory.CreatePlayer(name);
        }

        if (choice == "2")
        {
            Player player =
                SaveGame.Load();

            if (player == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Kein Spielstand gefunden.");

                Pause();

                return StartMenu();
            }

            Console.WriteLine(
                "Spielstand geladen.");

            Pause();

            return player;
        }

        return null;
    }

    // ========================================================
    // HAUPTMENÜ
    // ========================================================

    private static void MainMenu(
        Player player)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                $"{player.Name} | " +
                $"Level {player.Level} | " +
                $"HP {player.Health}/{player.MaxHealth} | " +
                $"Mana {player.Mana}/{player.MaxMana} | " +
                $"Gold {player.Gold}");

            Console.WriteLine(
                "==========================================");

            Console.WriteLine();

            Console.WriteLine("1. Orte erkunden");
            Console.WriteLine("2. Wald");
            Console.WriteLine("3. Schloss");
            Console.WriteLine("4. Inventar");
            Console.WriteLine("5. Quests");
            Console.WriteLine("6. Charakter");
            Console.WriteLine("7. Speichern");
            Console.WriteLine("8. Beenden");

            Console.WriteLine();
            Console.Write("Auswahl: ");

            string choice =
                ReadChoiceWithKeywords(
                    "Orte erkunden",
                    "Wald",
                    "Schloss",
                    "Inventar",
                    "Quests",
                    "Charakter",
                    "Speichern",
                    "Beenden");

            switch (choice)
            {
                case "1":
                    ExploreLocations(player);
                    break;

                case "2":
                    Forest(player);
                    break;

                case "3":
                    Castle(player);
                    break;

                case "4":
                    InventoryMenu(player);
                    break;

                case "5":
                    ViewQuestsHelper.ViewQuests(player);
                    break;

                case "6":
                    ShowCharacter(player);
                    break;

                case "7":

                    SaveGame.Save(player);

                    Console.WriteLine(
                        "Spiel gespeichert.");

                    Pause();

                    break;

                case "8":
                    running = false;
                    break;

                default:

                    Console.WriteLine(
                        "Ungültige Eingabe.");

                    Pause();

                    break;
            }
        }
    }

    // ========================================================
    // ORTE / EXPLORATION
    // ========================================================

    private static void ExploreLocations(Player player)
    {
        List<Location> locations = WorldFactory.GetAllLocations();

        while (true)
        {
            Console.Clear();

            Console.WriteLine("==========================================");
            Console.WriteLine("           ORTE ERKUNDEN");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            for (int i = 0; i < locations.Count; i++)
            {
                var loc = locations[i];
                Console.WriteLine($"{i + 1}. {loc.Name} (Level {loc.MinLevel}-{loc.MaxLevel})");
            }

            Console.WriteLine("0. Zurück");
            Console.WriteLine();
            Console.Write("Auswahl: ");

            // Optionen sind die Namen der Orte. Ermöglicht Texteingaben wie "ich gehe in den wald"
            string[] locOptions = new string[locations.Count];
            for (int i = 0; i < locations.Count; i++) locOptions[i] = locations[i].Name;

            string input = ReadChoiceWithKeywords(locOptions);

            // Falls der Benutzer "zurück" schreibt
            if (!string.IsNullOrWhiteSpace(input) && input.ToLowerInvariant().Contains("zurück")) return;

            if (input == "0") return;

            if (!int.TryParse(input, out int idx) || idx < 1 || idx > locations.Count)
            {
                Console.WriteLine("Ungültige Auswahl.");
                Pause();
                continue;
            }

            ExploreLocation(player, locations[idx - 1]);
        }
    }

    private static void ExploreLocation(Player player, Location location)
    {
        // Wenn dies eine definierte Stadt ist, zeige stadt-spezifische Optionen
        var town = StoryTexts.GetTownLocation(location.Name);

        if (town != null)
        {
            // Weise gegebenenfalls Quests zu (duplikate werden übersprungen)
            StoryTexts.AssignTownQuestsToPlayer(town.Name, player);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"== {town.Name} ==");
                Console.WriteLine(town.Description);
                Console.WriteLine();
                Console.WriteLine(StoryTexts.GetRandomTownEvent());
                Console.WriteLine();
                Console.WriteLine("1. Zum Laden gehen (Shop)");
                Console.WriteLine("2. Zur Taverne (gratis heilen)");
                Console.WriteLine("3. Quests ansehen / annehmen");
                Console.WriteLine("4. Zurück");
                Console.WriteLine();
                Console.Write("Auswahl: ");

                string choice = ReadChoiceWithKeywords(
                    "Laden",
                    "Taverne",
                    "Quests",
                    "Zurück");

                switch (choice)
                {
                    case "1":
                        // Shop
                        var inv = StoryTexts.GetShopInventory(town.Name).ToList();

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine($"== Laden von {town.Name} ==");
                            Console.WriteLine();
                            for (int i = 0; i < inv.Count; i++)
                            {
                                var it = inv[i];
                                Console.WriteLine($"{i + 1}. {it.Name} - {it.Description} ({it.Price} Gold)");
                            }

                            Console.WriteLine("0. Zurück");
                            Console.WriteLine();
                            Console.Write("Kaufen (Nummer): ");

                            string input = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(input)) break;

                            if (input == "0") break;

                            if (!int.TryParse(input, out int idx) || idx < 1 || idx > inv.Count)
                            {
                                Console.WriteLine("Ungültige Auswahl.");
                                Pause();
                                continue;
                            }

                            var shopItem = inv[idx - 1];

                            if (!player.SpendGold(shopItem.Price))
                            {
                                Console.WriteLine("Du hast nicht genug Gold.");
                                Pause();
                                continue;
                            }

                            // Erzeuge ein Item aus dem Shop-Eintrag (einfache Zuordnung)
                            Item newItem = CreateItemFromShopItem(shopItem);
                            player.Inventory.Add(newItem);

                            Console.WriteLine($"Du kaufst: {newItem.Name}");
                            Pause();
                        }

                        break;

                    case "2":
                        // Taverne: Gratis Heilung
                        Console.Clear();
                        Console.WriteLine(town.TavernDescription);
                        Console.WriteLine();
                        Console.WriteLine(town.TavernHealingText);
                        Console.WriteLine();
                        Console.WriteLine("Möchtest du dich ausruhen? (j/n)");
                        string yn = Console.ReadLine() ?? "n";

                        if (yn.Trim().Equals("j", StringComparison.OrdinalIgnoreCase) ||
                            yn.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            int healed = StoryTexts.UseTavernHealing(player, town.Name);
                            Console.WriteLine($"Du wurdest um {healed} HP geheilt und dein Mana wurde wiederhergestellt.");
                        }
                        else
                        {
                            Console.WriteLine("Du entscheidest dich weiterzureisen.");
                        }

                        Pause();
                        break;

                    case "3":
                        // Quests ansehen
                        var qTemplates = StoryTexts.GetAvailableQuests(town.Name).ToList();

                        Console.Clear();
                        Console.WriteLine($"== Quests in {town.Name} ==");
                        Console.WriteLine();

                        for (int i = 0; i < qTemplates.Count; i++)
                        {
                            var q = qTemplates[i];
                            Console.WriteLine($"{i + 1}. {q.Title} - {q.Description} (Geber: {q.Giver})");
                        }

                        Console.WriteLine();
                        Console.WriteLine("0. Zurück");
                        Console.WriteLine("Möchtest du alle Quests annehmen? (j/n)");
                        string accept = Console.ReadLine() ?? "n";

                        if (accept.Trim().Equals("j", StringComparison.OrdinalIgnoreCase) ||
                            accept.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            StoryTexts.AssignTownQuestsToPlayer(town.Name, player);
                            Console.WriteLine("Alle verfügbaren Quests wurden deinem Journal hinzugefügt.");
                        }

                        Pause();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        Pause();
                        break;
                }
            }
        }

        // Standard-Explore-Flow für Nicht-Städte
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"== {location.Name} ==");
            Console.WriteLine(location.Description);
            Console.WriteLine();
            Console.WriteLine($"Monster vor Ort: {location.Monsters.Count}");
            Console.WriteLine($"Gefundene Gegenstände: {location.Items.Count}");
            Console.WriteLine();
            Console.WriteLine("1. Nach Schätzen suchen");
            Console.WriteLine("2. Auf einen Gegner treffen (Kampf)");
            Console.WriteLine("3. Zurück");
            Console.WriteLine();
            Console.Write("Auswahl: ");

            string choice = ReadChoiceWithKeywords(
                "Nach Schätzen suchen",
                "Auf einen Gegner treffen",
                "Zurück");

            switch (choice)
            {
                case "1":
                    if (location.Items.Count == 0)
                    {
                        Console.WriteLine("Keine Gegenstände hier.");
                        Pause();
                        break;
                    }

                    int chance = Random.Shared.Next(1, 101);

                    if (chance <= 70)
                    {
                        int itemIndex = Random.Shared.Next(0, location.Items.Count);
                        Item found = location.Items[itemIndex];

                        player.Inventory.Add(found);

                        Console.WriteLine($"Du findest: {found.Name}");
                    }
                    else
                    {
                        Console.WriteLine("Du findest nichts von Wert.");
                    }

                    Pause();
                    break;

                case "2":
                    if (location.Monsters.Count == 0)
                    {
                        Console.WriteLine("Keine Monster hier.");
                        Pause();
                        break;
                    }

                    int mIndex = Random.Shared.Next(0, location.Monsters.Count);
                    Monster proto = location.Monsters[mIndex];
                    Monster enemy = CreateMonsterByName(proto.Name);

                    StartBattle(player, enemy);

                    if (!enemy.IsAlive())
                    {
                        int gold = enemy.GetGold();
                        player.AddGold(gold);
                        player.GainExperience(enemy.ExperienceReward);

                        foreach (var drop in enemy.GenerateDrops())
                        {
                            player.Inventory.Add(drop);
                        }

                        Console.WriteLine($"Du erhältst {enemy.ExperienceReward} XP und {gold} Gold.");
                        Pause();
                    }

                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("Ungültige Eingabe.");
                    Pause();
                    break;
            }
        }
    }

    private static Monster CreateMonsterByName(string name)
    {
        if (name.Contains("Goblin", StringComparison.OrdinalIgnoreCase))
            return GameFactory.CreateGoblin();

        if (name.Contains("Wolf", StringComparison.OrdinalIgnoreCase))
            return GameFactory.CreateWolf();

        if (name.Contains("Bandit", StringComparison.OrdinalIgnoreCase))
            return GameFactory.CreateBandit();

        if (name.Contains("Skelett", StringComparison.OrdinalIgnoreCase) || name.Contains("Skeleton", StringComparison.OrdinalIgnoreCase))
            return GameFactory.CreateSkeleton();

        if (name.Contains("Dunkler", StringComparison.OrdinalIgnoreCase) || name.Contains("Ritter", StringComparison.OrdinalIgnoreCase))
            return GameFactory.CreateDarkKnight();

        // Fallback
        return GameFactory.CreateGoblin();
    }

    private static Item CreateItemFromShopItem(StoryTexts.ShopItem shopItem)
    {
        if (shopItem == null)
            return new Item("Unbekannt", "Ein seltsamer Gegenstand.", ItemType.Material, shopItem?.Price ?? 1);

        string key = shopItem.Key ?? string.Empty;

        // Einfache Zuordnung basierend auf Schlüssel
        if (key.Contains("Health", StringComparison.OrdinalIgnoreCase) || key.Contains("Potion", StringComparison.OrdinalIgnoreCase))
        {
            return new Item(shopItem.Name, shopItem.Description, ItemType.Verbrauchbar, shopItem.Price, 30);
        }

        if (key.Contains("Sword", StringComparison.OrdinalIgnoreCase) || key.Contains("Iron", StringComparison.OrdinalIgnoreCase))
        {
            return new Item(shopItem.Name, shopItem.Description, ItemType.Waffe, shopItem.Price, 6);
        }

        if (key.Contains("Armor", StringComparison.OrdinalIgnoreCase) || key.Contains("Leather", StringComparison.OrdinalIgnoreCase))
        {
            return new Item(shopItem.Name, shopItem.Description, ItemType.Rüstung, shopItem.Price, 3);
        }

        // Default: Material
        return new Item(shopItem.Name, shopItem.Description, ItemType.Material, shopItem.Price);
    }

    // ========================================================
    // WALD
    // ========================================================

    private static void Forest(
        Player player)
    {
        Console.Clear();

        Console.WriteLine(
            "Du betrittst den dunklen Wald... Viel Glück!");

        Console.WriteLine();

        int eventRoll =
            Random.Shared.Next(1, 101);

        if (eventRoll <= 10)
        {
            Console.WriteLine(
                "Du läufst in eine vergiftete Falle!");

            player.ApplyPoison(4, 3);

            Console.WriteLine(
                "Du bist für 3 Runden vergiftet.");

            Pause();

            return;
        }

        if (eventRoll <= 20)
        {
            int gold =
                Random.Shared.Next(15, 51);

            player.AddGold(gold);

            Console.WriteLine(
                $"Du findest {gold} Gold.");

            Pause();

            return;
        }

        if (eventRoll <= 30)
        {
            Console.WriteLine(
                "Du findest einen versteckten Heiltrank.");

            player.Inventory.Add(
                new Item(
                    "Heiltrank",
                    "Heilt 30 HP.",
                    ItemType.Verbrauchbar,
                    20,
                    30));

            Pause();

            return;
        }

        Monster monster;

        int monsterRoll =
            Random.Shared.Next(1, 101);

        if (monsterRoll <= 35)
        {
            monster =
                GameFactory.CreateGoblin();
        }
        else if (monsterRoll <= 60)
        {
            monster =
                GameFactory.CreateWolf();
        }
        else
        {
            monster =
                GameFactory.CreateBandit();
        }

        StartBattle(
            player,
            monster);
    }

    // ========================================================
    // SCHLOSS
    // ========================================================

    private static void Castle(
        Player player)
    {
        Console.Clear();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "                  SCHLOSS");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine();

        Console.WriteLine("1. Eingangshalle");
        Console.WriteLine("2. Keller");
        Console.WriteLine("3. Obergeschoss");
        Console.WriteLine("4. Zurück");

        Console.WriteLine();
        Console.Write("Auswahl: ");

        string choice =
            ReadChoiceWithKeywords(
                "Eingangshalle",
                "Keller",
                "Obergeschoss",
                "Zurück");

        switch (choice)
        {
            case "1":

                StartBattle(
                    player,
                    GameFactory.CreateSkeleton());

                break;

            case "2":

                StartBattle(
                    player,
                    GameFactory.CreateSkeleton());

                break;

            case "3":

                StartBattle(
                    player,
                    GameFactory.CreateDarkKnight());

                break;

            case "4":
                return;

            default:

                Console.WriteLine(
                    "Ungültige Eingabe.");

                Pause();

                break;
        }
    }

    // ========================================================
    // KAMPF
    // ========================================================

    private static void StartBattle(
        Player player,
        Monster monster)
    {
        Console.Clear();

        Console.WriteLine(
            $"Ein {monster.Name} greift an!");

        Pause();

        while (
            player.IsAlive() &&
            monster.IsAlive())
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                $"DEIN STATUS: {player.Health}/{player.MaxHealth} HP");

            Console.WriteLine(
                $"MANA: {player.Mana}/{player.MaxMana}");

            Console.WriteLine();

            Console.WriteLine(
                $"GEGNER: {monster.Name}");

            Console.WriteLine(
                $"HP: {monster.Health}/{monster.MaxHealth}");

            Console.WriteLine(
                "==========================================");

            if (player.PoisonTurns > 0)
            {
                Console.WriteLine(
                    $"Gift: {player.PoisonTurns} Runde(n)");
            }

            Console.WriteLine();

            Console.WriteLine("1. Angriff");
            Console.WriteLine("2. Feuerball");
            Console.WriteLine("3. Heiltrank");
            Console.WriteLine("4. Fliehen");

            Console.WriteLine();
            Console.Write("Auswahl: ");

            string choice = ReadChoiceWithKeywords(
                "Angriff",
                "Feuerball",
                "Heiltrank",
                "Fliehen");

            bool successfulAction = true;

            switch (choice)
            {
                case "1":

                    AttackResult result =
                        CombatService.Attack(
                            player,
                            monster,
                            Random.Shared.Next);

                    if (result.Dodged)
                    {
                        Console.WriteLine(
                            $"{monster.Name} weicht deinem Angriff aus!");
                    }
                    else if (result.Critical)
                    {
                        Console.WriteLine(
                            $"KRITISCHER TREFFER! " +
                            $"{result.Damage} Schaden!");

                    }
                    else
                    {
                        Console.WriteLine(
                            $"Du verursachst " +
                            $"{result.Damage} Schaden!");
                    }

                    break;

                case "2":

                    int fireballDamage =
                        player.CastFireball(monster);

                    if (fireballDamage == 0)
                    {
                        Console.WriteLine(
                            "Nicht genug Mana!");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Der Feuerball verursacht " +
                            $"{fireballDamage} Schaden!");
                    }

                    break;

                case "3":

                    UsePotionMenu(player);

                    break;

                case "4":

                    int fleeRoll =
                        Random.Shared.Next(1, 101);

                    if (fleeRoll <= 50)
                    {
                        Console.WriteLine(
                            "Du bist erfolgreich geflohen.");

                        Pause();

                        return;
                    }

                    Console.WriteLine(
                        "Die Flucht ist fehlgeschlagen!");

                    break;

                default:

                    Console.WriteLine(
                        "Ungültige Eingabe.");

                    successfulAction = false;

                    break;
            }

            if (!successfulAction)
            {
                Pause();
                continue;
            }

            if (!monster.IsAlive())
            {
                break;
            }

            Pause();

            // ------------------------------------------------
            // STATUS-EFFEKTE
            // ------------------------------------------------

            int playerStatusDamage =
                player.ProcessStatusEffects();

            if (playerStatusDamage > 0)
            {
                Console.WriteLine(
                    $"Du erleidest " +
                    $"{playerStatusDamage} Statusschaden.");
            }

            if (!player.IsAlive())
            {
                break;
            }

            // ------------------------------------------------
            // GEGNERANGRIFF
            // ------------------------------------------------

            AttackResult enemyResult =
                CombatService.Attack(
                    monster,
                    player,
                    Random.Shared.Next);

            if (enemyResult.Dodged)
            {
                Console.WriteLine(
                    "Du weichst dem Angriff aus!");
            }
            else if (enemyResult.Critical)
            {
                Console.WriteLine(
                    $"KRITISCHER GEGENERANGRIFF! " +
                    $"{enemyResult.Damage} Schaden!");
            }
            else
            {
                Console.WriteLine(
                    $"{monster.Name} verursacht " +
                    $"{enemyResult.Damage} Schaden!");
            }

            // Wolf kann Gift verursachen
            if (
                monster.PoisonChance > 0 &&
                player.IsAlive())
            {
                int poisonRoll =
                    Random.Shared.Next(1, 101);

                if (poisonRoll <= monster.PoisonChance)
                {
                    player.ApplyPoison(3, 3);

                    Console.WriteLine(
                        "Du wurdest vergiftet!");
                }
            }

            Pause();
        }

        // ====================================================
        // SIEG
        // ====================================================

        if (player.IsAlive() &&
            !monster.IsAlive())
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                "                  SIEG!");

            Console.WriteLine(
                "==========================================");

            Console.WriteLine();

            Console.WriteLine(
                $"Du hast {monster.Name} besiegt.");

            int gold =
                monster.GetGold();

            player.AddGold(gold);

            player.GainExperience(
                monster.ExperienceReward);

            player.RegisterKill(
                monster.Name);

            Console.WriteLine(
                $"+{gold} Gold");

            Console.WriteLine(
                $"+{monster.ExperienceReward} XP");

            List<Item> drops =
                monster.GenerateDrops();

            foreach (Item item in drops)
            {
                Console.WriteLine(
                    $"Beute: {item.Name}");

                player.Inventory.Add(item);
            }

            Pause();
        }

        // ====================================================
        // TOD
        // ====================================================

        if (!player.IsAlive())
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                "                 BESIEGT");

            Console.WriteLine(
                "==========================================");

            Console.WriteLine();

            Console.WriteLine(
                "Du wurdest besiegt.");

            Console.WriteLine(
                "Du erwachst später mit der Hälfte deiner HP.");

            player.Health =
                player.MaxHealth / 2;

            if (player.Health < 1)
            {
                player.Health = 1;
            }

            Pause();
        }
    }

    // ========================================================
    // TRANK
    // ========================================================

    private static void UsePotionMenu(
        Player player)
    {
        List<int> potionIndexes =
            new();

        for (int i = 0;
             i < player.Inventory.Count;
             i++)
        {
            Item it = player.Inventory.GetAt(i);

            if (it != null && it.Type == ItemType.Verbrauchbar)
            {
                potionIndexes.Add(i);
            }
        }

        if (potionIndexes.Count == 0)
        {
            Console.WriteLine(
                "Du hast keine Heiltränke.");

            Pause();

            return;
        }

        Console.WriteLine();

        for (
            int i = 0;
            i < potionIndexes.Count;
            i++)
        {
            Item item =
                player.Inventory.GetAt(
                    potionIndexes[i]);

            Console.WriteLine(
                $"{i + 1}. {item.Name} " +
                $"(+{item.Power} HP)");
        }

        Console.WriteLine(
            "0. Abbrechen");

        Console.WriteLine();

        Console.Write("Auswahl: ");

        string input =
            Console.ReadLine();

        if (!int.TryParse(
                input,
                out int choice))
        {
            Console.WriteLine(
                "Ungültige Auswahl.");

            Pause();

            return;
        }

        if (choice == 0)
        {
            return;
        }

        if (
            choice < 1 ||
            choice > potionIndexes.Count)
        {
            Console.WriteLine(
                "Ungültige Auswahl.");

            Pause();

            return;
        }

        int realIndex =
            potionIndexes[choice - 1];

        int healed =
            player.UseHealingItem(
                realIndex);

        Console.WriteLine(
            $"Du hast {healed} HP geheilt.");

        Pause();
    }

    // ========================================================
    // INVENTAR
    // ========================================================

    private static void InventoryMenu(
        Player player)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                "                 INVENTAR");

            Console.WriteLine(
                "==========================================");

            if (player.Inventory.Count == 0)
            {
                Console.WriteLine(
                    "Dein Inventar ist leer.");
            }
            else
            {
                for (
                    int i = 0;
                    i < player.Inventory.Count;
                    i++)
                {
                    Item invItem = player.Inventory.GetAt(i);

                    if (invItem == null)
                    {
                        continue;
                    }

                    Console.WriteLine(
                        $"{i + 1}. {invItem.Name} | " +
                        $"{invItem.Type} | " +
                        $"Power: {invItem.Power}");
                }
            }

            Console.WriteLine();

            Console.WriteLine(
                "1. Item ausrüsten");

            Console.WriteLine(
                "2. Item verkaufen");

            Console.WriteLine(
                "3. Zurück");

            Console.WriteLine();

            Console.Write("Auswahl: ");

            string choice = ReadChoiceWithKeywords(
                "Item ausrüsten",
                "Item verkaufen",
                "Zurück");

            if (choice == "3")
            {
                return;
            }

            if (choice != "1" && choice != "2")
            {
                continue;
            }

            Console.WriteLine();

            Console.Write("Nummer des Items: ");

            // Optionen sind die Item-Namen, erlauben Auswahl per Text
            string[] itemOptions = new string[player.Inventory.Count];
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                var it = player.Inventory.GetAt(i);
                itemOptions[i] = it?.Name ?? string.Empty;
            }

            string itemInput = ReadChoiceWithKeywords(itemOptions);

            if (!string.IsNullOrWhiteSpace(itemInput) && itemInput.ToLowerInvariant().Contains("zurück"))
            {
                continue;
            }

            int index;
            if (!int.TryParse(itemInput, out index))
            {
                // Versuche anhand des Namens zu matchen
                string lower = itemInput?.ToLowerInvariant() ?? string.Empty;
                index = -1;

                for (int i = 0; i < itemOptions.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(itemOptions[i]) && lower.Contains(itemOptions[i].ToLowerInvariant()))
                    {
                        index = i + 1;
                        break;
                    }
                }

                if (index == -1)
                {
                    continue;
                }
            }

            index--;

            Item selectedItem = player.Inventory.GetAt(index);

            if (selectedItem == null)
            {
                Console.WriteLine("Item nicht gefunden.");

                Pause();

                continue;
            }

            if (choice == "1")
            {
                if (player.Equip(selectedItem))
                {
                    Console.WriteLine($"{selectedItem.Name} ausgerüstet.");
                }
                else
                {
                    Console.WriteLine("Dieses Item kann nicht ausgerüstet werden.");
                }

                Pause();
                continue;
            }

            // Verkauf
            int sold = player.SellItem(index);

            if (sold <= 0)
            {
                Console.WriteLine("Dieses Item kann nicht verkauft werden.");
            }
            else
            {
                Console.WriteLine($"Du hast {sold} Gold für {selectedItem.Name} erhalten.");
            }

            Pause();
        }
    }

    // ========================================================
    // QUESTS
    // ========================================================

    private static void QuestMenu(
        Player player)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                "                  QUESTS");

            Console.WriteLine(
                "==========================================");

            for (
                int i = 0;
                i < player.Quests.Count;
                i++)
            {
                Quest quest =
                    player.Quests[i];

                string status;

                if (quest.Claimed)
                {
                    status = "ABGESCHLOSSEN";
                }
                else if (quest.IsCompleted())
                {
                    status = "BELohnung verfügbar";
                }
                else
                {
                    status =
                        $"{quest.CurrentKills}/" +
                        $"{quest.RequiredKills}";
                }

                Console.WriteLine();
                Console.WriteLine(
                    $"{i + 1}. {quest.Name}");

                Console.WriteLine(
                    $"   {quest.Description}");

                Console.WriteLine(
                    $"   Fortschritt: {status}");

                Console.WriteLine(
                    $"   Belohnung: " +
                    $"{quest.RewardXP} XP + " +
                    $"{quest.RewardGold} Gold");
            }

            Console.WriteLine();

            Console.WriteLine(
                "0. Zurück");

            Console.WriteLine(
                "1. Belohnungen einsammeln");

        Console.WriteLine();

        Console.Write("Auswahl: ");

        string choice = ReadChoiceWithKeywords(
            "Eingangshalle",
            "Keller",
            "Obergeschoss",
            "Zurück");

            if (choice == "0")
            {
                return;
            }

            if (choice != "1")
            {
                continue;
            }

            foreach (Quest quest in player.Quests)
            {
                if (
                    quest.IsCompleted() &&
                    !quest.Claimed)
                {
                    quest.Claim(player);

                    Console.WriteLine();
                    Console.WriteLine(
                        $"Quest abgeschlossen: " +
                        $"{quest.Name}");

                    Console.WriteLine(
                        $"+{quest.RewardXP} XP");

                    Console.WriteLine(
                        $"+{quest.RewardGold} Gold");
                }
            }

            Pause();
        }
    }

    // ========================================================
    // SHOP
    // ========================================================

    private static void Shop(
        Player player)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                "                   SHOP");

            Console.WriteLine(
                "==========================================");

            Console.WriteLine(
                $"Gold: {player.Gold}");

            Console.WriteLine();

            Console.WriteLine(
                "1. Heiltrank       - 20 Gold");

            Console.WriteLine(
                "2. Großer Heiltrank - 40 Gold");

            Console.WriteLine(
                "3. Eisenschwert    - 80 Gold");

            Console.WriteLine(
                "4. Kettenrüstung   - 70 Gold");

            Console.WriteLine(
                "5. Zurück");

            Console.WriteLine();

            Console.Write("Auswahl: ");

            string choice = ReadChoiceWithKeywords(
                "Heiltrank",
                "Großer Heiltrank",
                "Eisenschwert",
                "Kettenrüstung",
                "Zurück");

            if (choice == "5")
            {
                return;
            }

            Item item = null;

            switch (choice)
            {
                case "1":

                    item = new Item(
                        "Heiltrank",
                        "Heilt 30 HP.",
                        ItemType.Verbrauchbar,
                        20,
                        30);

                    break;

                case "2":

                    item = new Item(
                        "Großer Heiltrank",
                        "Heilt 70 HP.",
                        ItemType.Verbrauchbar,
                        40,
                        70);

                    break;

                case "3":

                    item = new Item(
                        "Eisenschwert",
                        "Ein starkes Schwert.",
                        ItemType.Waffe,
                        80,
                        8);

                    break;

                case "4":

                    item = new Item(
                        "Kettenrüstung",
                        "Eine starke Rüstung.",
                        ItemType.Rüstung,
                        70,
                        6);

                    break;

                default:

                    Console.WriteLine(
                        "Ungültige Auswahl.");

                    Pause();

                    continue;
            }

            if (!player.SpendGold(item.Price))
            {
                Console.WriteLine(
                    "Nicht genug Gold.");

                Pause();

                continue;
            }

            player.Inventory.Add(item);

            Console.WriteLine(
                $"{item.Name} gekauft!");

            Pause();
        }
    }

    // ========================================================
    // CHARACTER
    // ========================================================

    private static void ShowCharacter(
        Player player)
    {
        Console.Clear();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "               CHARAKTER");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine();

        Console.WriteLine(
            $"Name:       {player.Name}");

        Console.WriteLine(
            $"Level:      {player.Level}");

        Console.WriteLine(
            $"XP:         {player.Experience}/" +
            $"{player.RequiredExperience()}");

        Console.WriteLine(
            $"HP:         {player.Health}/" +
            $"{player.MaxHealth}");

        Console.WriteLine(
            $"Mana:       {player.Mana}/" +
            $"{player.MaxMana}");

        Console.WriteLine(
            $"Angriff:    {player.Attack}");

        Console.WriteLine(
            $"Verteid.:   {player.Defense}");

        Console.WriteLine(
            $"Krit-Chance:{player.CriticalChance}%");

        Console.WriteLine(
            $"Ausweichen: {player.DodgeChance}%");

        Console.WriteLine(
            $"Gold:       {player.Gold}");

        Console.WriteLine();

        Console.WriteLine(
            $"Waffe:      " +
            $"{(string.IsNullOrEmpty(player.EquippedWeapon) ? "Keine" : player.EquippedWeapon)}");

        Console.WriteLine(
            $"Rüstung:    " +
            $"{(string.IsNullOrEmpty(player.EquippedArmor) ? "Keine" : player.EquippedArmor)}");

        Console.WriteLine();

        Console.WriteLine(
            "Besiegte Gegner:");

        foreach (var statistic in player.KillStatistics)
        {
            Console.WriteLine(
                $"- {statistic.Key}: " +
                $"{statistic.Value}");
        }
        Console.WriteLine();

        Console.WriteLine($"Skillpunkte: {player.SkillPoints}");

        if (player.SkillPoints > 0)
        {
            Console.WriteLine();
            Console.WriteLine("1. Skillpunkte verteilen");
            Console.WriteLine("2. Zurück");
            Console.WriteLine();
            Console.Write("Auswahl: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                AllocateSkillPoints(player);
            }
        }
        else
        {
            Pause();
        }
    }

    // ========================================================
    // PAUSE
    // ========================================================

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine(
            "Drücke eine Taste, um fortzufahren...");

        Console.ReadKey();
    }

    // ========================================================
    // BENUTZER-EINGABE MIT STICHWORT-ERKENNUNG
    // ========================================================

    private static string ReadChoiceWithKeywords(params string[] options)
    {
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        input = input.Trim();

        // Direkte Zahleneingabe bevorzugen
        if (int.TryParse(input, out _))
        {
            return input;
        }

        string lower = input.ToLowerInvariant();

        // Tokenize Eingabe
        var tokens = System.Text.RegularExpressions.Regex.Split(lower, "\\W+");

        // Synonyme-Mapping (kleine, erweiterbare Liste)
        var synonyms = new System.Collections.Generic.Dictionary<string, string[]>()
        {
            { "gehen", new[] { "erkunden", "orte", "wald", "schloss" } },
            { "gehe", new[] { "erkunden", "orte", "wald", "schloss" } },
            { "betreten", new[] { "erkunden", "orte", "wald", "schloss" } },
            { "betrete", new[] { "erkunden", "orte", "wald", "schloss" } },
            { "kaufen", new[] { "shop", "shoppen" } },
            { "kaufe", new[] { "shop", "shoppen" } },
            { "shoppen", new[] { "shop" } },
            { "inventar", new[] { "inventar" } },
            { "quest", new[] { "quests" } },
            { "quests", new[] { "quests" } },
            { "charakter", new[] { "charakter" } },
            { "speichern", new[] { "speichern" } },
            { "laden", new[] { "laden" } },
            { "beenden", new[] { "beenden" } },
            { "angriff", new[] { "angriff" } },
            { "attacke", new[] { "angriff" } },
            { "feuerball", new[] { "feuerball" } },
            { "heiltrank", new[] { "heiltrank" } },
            { "trank", new[] { "heiltrank" } },
            { "heilen", new[] { "heiltrank" } },
            { "fliehen", new[] { "fliehen" } },
            { "flucht", new[] { "fliehen" } },
            { "ausrüsten", new[] { "ausrüsten" } },
            { "ausruesten", new[] { "ausrüsten" } },
            { "verkaufen", new[] { "verkaufen", "shop" } },
            { "zurück", new[] { "zurück", "abbrechen" } },
            { "zurueck", new[] { "zurück", "abbrechen" } },
        };

        // Helper: Levenshtein-Distanz
        static int Levenshtein(string a, string b)
        {
            if (a == b) return 0;
            if (string.IsNullOrEmpty(a)) return b.Length;
            if (string.IsNullOrEmpty(b)) return a.Length;
            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) d[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[a.Length, b.Length];
        }

        for (int i = 0; i < options.Length; i++)
        {
            string opt = options[i] ?? string.Empty;
            string optLower = opt.ToLowerInvariant();

            // 1) Direkte vollständige Bezeichnung
            if (!string.IsNullOrWhiteSpace(optLower) && lower.Contains(optLower))
            {
                return (i + 1).ToString();
            }

            // 2) Synonym-Check: Falls ein Token eine bekannte Synonym-Mapping hat und das Ziel im Optionstext vorkommt
            foreach (var t in tokens)
            {
                if (string.IsNullOrWhiteSpace(t)) continue;

                if (synonyms.TryGetValue(t, out var targets))
                {
                    foreach (var target in targets)
                    {
                        if (optLower.Contains(target))
                        {
                            return (i + 1).ToString();
                        }
                    }
                }
            }

            // 3) Einzelne Wörter der Option prüfen (exakt oder fuzzy)
            var words = optLower.Split(new[] { ' ', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var w in words)
            {
                if (string.IsNullOrWhiteSpace(w)) continue;

                // exakte enthalten-Überprüfung
                foreach (var t in tokens)
                {
                    if (string.IsNullOrWhiteSpace(t)) continue;

                    if (t.Contains(w) || w.Contains(t))
                    {
                        return (i + 1).ToString();
                    }

                    // fuzzy: kleine Tippfehler zulassen
                    int dist = Levenshtein(t, w);
                    int threshold = w.Length <= 4 ? 1 : Math.Max(1, w.Length / 4);
                    if (dist <= threshold)
                    {
                        return (i + 1).ToString();
                    }
                }
            }
        }

        // Falls nichts passt, gib die rohe Eingabe zurück
        return input;
    }

    // ========================================================
    // SKILL-ALLOCATION
    // ========================================================

    private static void AllocateSkillPoints(Player player)
    {
        while (player.SkillPoints > 0)
        {
            Console.Clear();

            Console.WriteLine("Skillpunkte: " + player.SkillPoints);
            Console.WriteLine();
            Console.WriteLine("1. +MaxHealth (+10)");
            Console.WriteLine("2. +MaxMana (+5)");
            Console.WriteLine("3. +Angriff (+1)");
            Console.WriteLine("4. +Verteidigung (+1)");
            Console.WriteLine("5. +Krit-Chance (+1)");
            Console.WriteLine("6. +Ausweichen (+1)");
            Console.WriteLine("7. Beenden");
            Console.WriteLine();
            Console.Write("Auswahl: ");

            string input = ReadChoiceWithKeywords(
                "+MaxHealth",
                "+MaxMana",
                "+Angriff",
                "+Verteidigung",
                "+Krit-Chance",
                "+Ausweichen",
                "Beenden");

            switch (input)
            {
                case "1":
                    player.SpendSkillPointOnHealth();
                    break;

                case "2":
                    player.SpendSkillPointOnMana();
                    break;

                case "3":
                    player.SpendSkillPointOnAttack();
                    break;

                case "4":
                    player.SpendSkillPointOnDefense();
                    break;

                case "5":
                    player.SpendSkillPointOnCritical();
                    break;

                case "6":
                    player.SpendSkillPointOnDodge();
                    break;

                case "7":
                    return;

                default:
                    Console.WriteLine("Ungültige Eingabe.");
                    Pause();
                    break;
            }

            player.UpdateStats();
        }
    }
}