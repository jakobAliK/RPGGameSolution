namespace RPGGame;

// ============================================================
// LOCATION / ORT
// ============================================================

public class Location
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public int MinLevel { get; set; } = 1;
    public int MaxLevel { get; set; } = 10;

    public List<Item> Items { get; private set; } = new();
    public List<Monster> Monsters { get; private set; } = new();

    public Location()
    {
    }

    public Location(string name, string description, int minLevel, int maxLevel)
    {
        Name = name;
        Description = description;
        MinLevel = minLevel;
        MaxLevel = maxLevel;
    }

    public void AddItem(Item item)
    {
        if (item == null) return;
        Items.Add(item);
    }

    public void AddMonster(Monster monster)
    {
        if (monster == null) return;
        Monsters.Add(monster);
    }
}
