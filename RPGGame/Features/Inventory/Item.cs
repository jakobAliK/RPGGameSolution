namespace RPGGame;

// ============================================================
// ITEM
// ============================================================

public class Item
{
    [System.Text.Json.Serialization.JsonInclude]
    public string Name { get; private set; } = "";

    [System.Text.Json.Serialization.JsonInclude]
    public string Description { get; private set; } = "";

    [System.Text.Json.Serialization.JsonInclude]
    public ItemType Type { get; private set; }

    [System.Text.Json.Serialization.JsonInclude]
    public int Price { get; private set; }

    [System.Text.Json.Serialization.JsonInclude]
    public int Power { get; private set; }

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
