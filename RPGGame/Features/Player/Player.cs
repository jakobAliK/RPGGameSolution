using System;
using System.Collections.ObjectModel;

namespace RPGGame;

// ============================================================
// PLAYER
// ============================================================

public partial class Player : Character
{
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }

    // Skill points that can be spent on passive boosts
    public int SkillPoints { get; private set; }

    public int Gold { get; private set; } = 50;

    public int MaxMana { get; private set; } = 50;
    public int Mana { get; private set; } = 50;

    public int BaseAttack { get; private set; } = 10;
    public int BaseDefense { get; private set; } = 5;

    public string EquippedWeapon { get; private set; } = "";
    public string EquippedArmor { get; private set; } = "";

    [System.Text.Json.Serialization.JsonInclude]
    public Inventory Inventory { get; internal set; } = new();

    [System.Text.Json.Serialization.JsonInclude]
    public List<Quest> Quests { get; internal set; } = new();

    [System.Text.Json.Serialization.JsonInclude]
    public Dictionary<string, int> KillStatistics { get; internal set; } = new();

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
        // Progressiverer XP-Bedarf: quadratisch
        return 100 * Level * Level;
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

        // Größere Zuwächse pro Level
        MaxHealth += 30;
        Health = MaxHealth;

        MaxMana += 15;
        Mana = MaxMana;

        BaseAttack += 4;
        BaseDefense += 3;

        // Leichter Anstieg der Treffer-/Krit-Werte
        CriticalChance += 1;
        DodgeChance += 1;

        // Belohne den Spieler mit Skill-Punkten
        SkillPoints += 1;

        UpdateStats();
    }

    // --------------------------------------------------------
    // ECONOMY & QUESTS
    // --------------------------------------------------------

    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        return true;
    }

    public void AddQuest(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        Quests.Add(quest);
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
    // TAVERNE - GRATIS HEILUNG
    // --------------------------------------------------------

    public int RestAtTavern()
    {
        int amountToHeal = Math.Max(0, MaxHealth - Health);

        if (amountToHeal <= 0)
        {
            // Already full health
            // Still restore mana
            Mana = MaxMana;
            return 0;
        }

        // Heile den Spieler komplett und fülle Mana auf
        Heal(amountToHeal);
        Mana = MaxMana;

        return amountToHeal;
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

        AddGold(sellPrice);

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

// --------------------------------------------------------
// SKILL-POINTS (öffentliche Methoden zum Verteilen)
// --------------------------------------------------------

public partial class Player
{
    public bool SpendSkillPointOnHealth()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        MaxHealth += 10;
        Health += 10;
        return true;
    }

    public bool SpendSkillPointOnMana()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        MaxMana += 5;
        Mana += 5;
        return true;
    }

    public bool SpendSkillPointOnAttack()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        BaseAttack += 1;
        UpdateStats();
        return true;
    }

    public bool SpendSkillPointOnDefense()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        BaseDefense += 1;
        UpdateStats();
        return true;
    }

    public bool SpendSkillPointOnCritical()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        CriticalChance += 1;
        return true;
    }

    public bool SpendSkillPointOnDodge()
    {
        if (SkillPoints <= 0) return false;

        SkillPoints -= 1;
        DodgeChance += 1;
        return true;
    }
}
