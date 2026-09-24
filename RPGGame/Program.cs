using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RPGGame;

// ============================================================
// ENUMS
// ============================================================

public enum ItemType
{
    Verbrauchbar,
    Waffe,
    Rüstung,
    Material
}

public enum StatusEffectType
{
    Keine,
    Gift,
    Brennen
}


// ============================================================
// ITEM
// ============================================================

public class Item
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ItemType Type { get; set; }
    public int Price { get; set; }
    public int Power { get; set; }

    public Item()
    {
    }

    public Item(
        string name,
        string description,
        ItemType type,
        int price,
        int power = 0)
    {
        Name = name;
        Description = description;
        Type = type;
        Price = price;
        Power = power;
    }
}


// ============================================================
// INVENTAR
// ============================================================

public class Inventory
{
    public List<Item> Items { get; set; } = new();

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public bool Remove(Item item)
    {
        return Items.Remove(item);
    }

    public Item Find(string name)
    {
        foreach (Item item in Items)
        {
            if (item.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }

        return null;
    }

    public Item GetAt(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            return null;
        }

        return Items[index];
    }

    public int CountType(ItemType type)
    {
        int count = 0;

        foreach (Item item in Items)
        {
            if (item.Type == type)
            {
                count++;
            }
        }

        return count;
    }
}


// ============================================================
// QUEST
// ============================================================

public class Quest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public string TargetMonster { get; set; } = "";

    public int RequiredKills { get; set; }
    public int CurrentKills { get; set; }

    public int RewardXP { get; set; }
    public int RewardGold { get; set; }

    public bool Claimed { get; set; }

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
        player.Gold += RewardGold;

        return true;
    }
}


// ============================================================
// CHARACTER
// ============================================================

public class Character
{
    public string Name { get; set; } = "";

    public int MaxHealth { get; set; }
    public int Health { get; set; }

    public int Attack { get; set; }
    public int Defense { get; set; }

    public int DodgeChance { get; set; }
    public int CriticalChance { get; set; }

    public int PoisonTurns { get; set; }
    public int PoisonDamage { get; set; }

    public int BurnTurns { get; set; }
    public int BurnDamage { get; set; }

    public Character()
    {
    }

    public Character(
        string name,
        int maxHealth,
        int attack,
        int defense)
    {
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
        Attack = attack;
        Defense = defense;

        DodgeChance = 5;
        CriticalChance = 10;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    public int TakeDamage(int rawDamage)
    {
        int finalDamage = Math.Max(
            1,
            rawDamage - Defense);

        Health -= finalDamage;

        if (Health < 0)
        {
            Health = 0;
        }

        return finalDamage;
    }

    public int Heal(int amount)
    {
        int oldHealth = Health;

        Health += amount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        return Health - oldHealth;
    }

    public void ApplyPoison(
        int damage,
        int turns)
    {
        PoisonDamage = damage;
        PoisonTurns = Math.Max(
            PoisonTurns,
            turns);
    }

    public void ApplyBurn(
        int damage,
        int turns)
    {
        BurnDamage = damage;
        BurnTurns = Math.Max(
            BurnTurns,
            turns);
    }

    public int ProcessStatusEffects()
    {
        int totalDamage = 0;

        if (PoisonTurns > 0)
        {
            Health -= PoisonDamage;

            if (Health < 0)
            {
                Health = 0;
            }

            totalDamage += PoisonDamage;

            PoisonTurns--;
        }

        if (BurnTurns > 0)
        {
            Health -= BurnDamage;

            if (Health < 0)
            {
                Health = 0;
            }

            totalDamage += BurnDamage;

            BurnTurns--;
        }

        return totalDamage;
    }
}


// ============================================================
// PLAYER
// ============================================================

public class Player : Character
{
    public int Level { get; set; } = 1;
    public int Experience { get; set; }

    public int Gold { get; set; } = 50;

    public int MaxMana { get; set; } = 50;
    public int Mana { get; set; } = 50;

    public int BaseAttack { get; set; } = 10;
    public int BaseDefense { get; set; } = 5;

    public string EquippedWeapon { get; set; } = "";
    public string EquippedArmor { get; set; } = "";

    public Inventory Inventory { get; set; } = new();

    public List<Quest> Quests { get; set; } = new();

    public Dictionary<string, int> KillStatistics { get; set; }
        = new();

    public Player()
    {
    }

    public Player(string name)
        : base(name, 100, 10, 5)
    {
        BaseAttack = 10;
        BaseDefense = 5;

        MaxMana = 50;
        Mana = MaxMana;

        CriticalChance = 10;
        DodgeChance = 5;
    }

    // --------------------------------------------------------
    // XP
    // --------------------------------------------------------

    public int RequiredExperience()
    {
        return 100 + ((Level - 1) * 50);
    }

    public void GainExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Experience += amount;

        while (Experience >= RequiredExperience())
        {
            Experience -= RequiredExperience();

            LevelUp();
        }
    }

    public void LevelUp()
    {
        Level++;

        MaxHealth += 20;
        Health = MaxHealth;

        MaxMana += 10;
        Mana = MaxMana;

        BaseAttack += 3;
        BaseDefense += 2;

        CriticalChance += 1;
        DodgeChance += 1;

        UpdateStats();
    }

    // --------------------------------------------------------
    // STATS
    // --------------------------------------------------------

    public void UpdateStats()
    {
        Attack = BaseAttack;
        Defense = BaseDefense;

        Item weapon = Inventory.Find(EquippedWeapon);

        if (weapon != null &&
            weapon.Type == ItemType.Waffe)
        {
            Attack += weapon.Power;
        }

        Item armor = Inventory.Find(EquippedArmor);

        if (armor != null &&
            armor.Type == ItemType.Rüstung)
        {
            Defense += armor.Power;
        }
    }

    // --------------------------------------------------------
    // AUSRÜSTEN
    // --------------------------------------------------------

    public bool Equip(Item item)
    {
        if (item == null)
        {
            return false;
        }

        if (!Inventory.Items.Contains(item))
        {
            return false;
        }

        if (item.Type == ItemType.Waffe)
        {
            EquippedWeapon = item.Name;
            UpdateStats();
            return true;
        }

        if (item.Type == ItemType.Rüstung)
        {
            EquippedArmor = item.Name;
            UpdateStats();
            return true;
        }

        return false;
    }

    // --------------------------------------------------------
    // HEILTRANK
    // --------------------------------------------------------

    public int UseHealingItem(int index)
    {
        Item item = Inventory.GetAt(index);

        if (item == null)
        {
            return 0;
        }

        if (item.Type != ItemType.Verbrauchbar)
        {
            return 0;
        }

        int healed = Heal(item.Power);

        Inventory.Remove(item);

        return healed;
    }

    // --------------------------------------------------------
    // VERKAUF
    // --------------------------------------------------------

    public int SellItem(int index)
    {
        Item item = Inventory.GetAt(index);

        if (item == null)
        {
            return 0;
        }

        // Prevent selling quest items? (simple rule: materials and consumables and equipment can be sold)
        // Unequip if currently equipped
        if (!string.IsNullOrEmpty(EquippedWeapon) && item.Name == EquippedWeapon)
        {
            EquippedWeapon = string.Empty;
        }

        if (!string.IsNullOrEmpty(EquippedArmor) && item.Name == EquippedArmor)
        {
            EquippedArmor = string.Empty;
        }

        int sellPrice = Math.Max(1, item.Price / 2);

        bool removed = Inventory.Remove(item);

        if (!removed)
        {
            return 0;
        }

        Gold += sellPrice;

        UpdateStats();

        return sellPrice;
    }

    // --------------------------------------------------------
    // FIREBALL
    // --------------------------------------------------------

    public int CastFireball(Monster target)
    {
        const int manaCost = 20;

        if (Mana < manaCost)
        {
            return 0;
        }

        Mana -= manaCost;

        int damage =
            25 +
            (Level * 5);

        int finalDamage =
            Math.Max(
                1,
                damage - (target.Defense / 2));

        target.Health -= finalDamage;

        if (target.Health < 0)
        {
            target.Health = 0;
        }

        return finalDamage;
    }

    // --------------------------------------------------------
    // QUEST-KILL
    // --------------------------------------------------------

    public void RegisterKill(string monsterName)
    {
        if (KillStatistics.ContainsKey(monsterName))
        {
            KillStatistics[monsterName]++;
        }
        else
        {
            KillStatistics[monsterName] = 1;
        }

        foreach (Quest quest in Quests)
        {
            quest.RegisterKill(monsterName);
        }
    }
}


// ============================================================
// MONSTER
// ============================================================

public class Monster : Character
{
    public int ExperienceReward { get; set; }

    public int MinGold { get; set; }
    public int MaxGold { get; set; }

    public int PoisonChance { get; set; }

    public List<Item> Drops { get; set; } = new();

    public Monster()
    {
    }

    public Monster(
        string name,
        int health,
        int attack,
        int defense,
        int experienceReward,
        int minGold,
        int maxGold)
        : base(name, health, attack, defense)
    {
        ExperienceReward = experienceReward;
        MinGold = minGold;
        MaxGold = maxGold;
    }

    public int GetGold()
    {
        return Random.Shared.Next(
            MinGold,
            MaxGold + 1);
    }

    public List<Item> GenerateDrops()
    {
        List<Item> result = new();

        foreach (Item item in Drops)
        {
            int chance = Random.Shared.Next(1, 101);

            if (chance <= 50)
            {
                result.Add(item);
            }
        }

        return result;
    }
}


// ============================================================
// COMBAT RESULT
// ============================================================

public class AttackResult
{
    public bool Dodged { get; set; }
    public bool Critical { get; set; }
    public int Damage { get; set; }
}


// ============================================================
// COMBAT SERVICE
// ============================================================

public static class CombatService
{
    public static AttackResult Attack(
        Character attacker,
        Character defender,
        Func<int, int, int> random)
    {
        AttackResult result = new();

        int dodgeRoll =
            random(1, 101);

        if (dodgeRoll <= defender.DodgeChance)
        {
            result.Dodged = true;
            result.Damage = 0;

            return result;
        }

        int critRoll =
            random(1, 101);

        result.Critical =
            critRoll <= attacker.CriticalChance;

        int rawDamage = attacker.Attack;

        if (result.Critical)
        {
            rawDamage *= 2;
        }

        result.Damage =
            defender.TakeDamage(rawDamage);

        return result;
    }
}


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
        player.Quests.Add(
            new Quest(
                "Goblin-Jäger",
                "Besiege 3 Goblins.",
                "Goblin",
                3,
                100,
                50));

        player.Quests.Add(
            new Quest(
                "Wolfjäger",
                "Besiege 2 Wölfe.",
                "Wolf",
                2,
                120,
                70));

        player.Quests.Add(
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

        monster.Drops.Add(
            new Item(
                "Goblin-Dolch",
                "Ein rostiger Dolch.",
                ItemType.Waffe,
                25,
                2));

        monster.Drops.Add(
            new Item(
                "Heiltrank",
                "Heilt 30 HP.",
                ItemType.Verbrauchbar,
                20,
                30));

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

        monster.Drops.Add(
            new Item(
                "Wolfspelz",
                "Ein wertvoller Pelz.",
                ItemType.Material,
                15));

        monster.Drops.Add(
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

        monster.Drops.Add(
            new Item(
                "Banditenschwert",
                "Ein gebrauchtes Schwert.",
                ItemType.Waffe,
                50,
                5));

        monster.Drops.Add(
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

        monster.Drops.Add(
            new Item(
                "Knochenrüstung",
                "Eine ungewöhnliche Rüstung.",
                ItemType.Rüstung,
                60,
                5));

        monster.Drops.Add(
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

        monster.Drops.Add(
            new Item(
                "Dunkelschwert",
                "Das Schwert des dunklen Ritters.",
                ItemType.Waffe,
                150,
                12));

        monster.Drops.Add(
            new Item(
                "Ritterrüstung",
                "Eine mächtige Rüstung.",
                ItemType.Rüstung,
                140,
                10));

        monster.Drops.Add(
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
            Console.ReadLine();

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

            Console.WriteLine("1. Wald");
            Console.WriteLine("2. Schloss");
            Console.WriteLine("3. Shop");
            Console.WriteLine("4. Inventar");
            Console.WriteLine("5. Quests");
            Console.WriteLine("6. Charakter");
            Console.WriteLine("7. Speichern");
            Console.WriteLine("8. Beenden");

            Console.WriteLine();
            Console.Write("Auswahl: ");

            string choice =
                Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Forest(player);
                    break;

                case "2":
                    Castle(player);
                    break;

                case "3":
                    Shop(player);
                    break;

                case "4":
                    InventoryMenu(player);
                    break;

                case "5":
                    QuestMenu(player);
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
    // WALD
    // ========================================================

    private static void Forest(
        Player player)
    {
        Console.Clear();

        Console.WriteLine(
            "Du betrittst den dunklen Wald...");

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

            player.Gold += gold;

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
            Console.ReadLine();

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

            string choice =
                Console.ReadLine();

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

            player.Gold += gold;

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
             i < player.Inventory.Items.Count;
             i++)
        {
            if (
                player.Inventory.Items[i].Type ==
                ItemType.Verbrauchbar)
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
                player.Inventory.Items[
                    potionIndexes[i]];

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

            if (player.Inventory.Items.Count == 0)
            {
                Console.WriteLine(
                    "Dein Inventar ist leer.");
            }
            else
            {
                for (
                    int i = 0;
                    i < player.Inventory.Items.Count;
                    i++)
                {
                    Item invItem =
                        player.Inventory.Items[i];

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

            string choice = Console.ReadLine();

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

            if (!int.TryParse(Console.ReadLine(), out int index))
            {
                continue;
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

            string choice =
                Console.ReadLine();

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

            string choice =
                Console.ReadLine();

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

            if (player.Gold < item.Price)
            {
                Console.WriteLine(
                    "Nicht genug Gold.");

                Pause();

                continue;
            }

            player.Gold -= item.Price;

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

        Pause();
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
}