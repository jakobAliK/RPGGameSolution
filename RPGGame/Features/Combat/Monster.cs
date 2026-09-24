namespace RPGGame;

// ============================================================
// MONSTER
// ============================================================

public class Monster : Character
{
    public int ExperienceReward { get; private set; }

    public int MinGold { get; private set; }
    public int MaxGold { get; private set; }

    public int PoisonChance { get; set; }

    public List<Item> Drops { get; private set; } = new();

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

    public void AddDrop(Item item)
    {
        if (item == null)
        {
            return;
        }

        Drops.Add(item);
    }
}
