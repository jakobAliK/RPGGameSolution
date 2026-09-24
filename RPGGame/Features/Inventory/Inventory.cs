namespace RPGGame;

// ============================================================
// INVENTAR
// ============================================================

public class Inventory
{
    [System.Text.Json.Serialization.JsonInclude]
    public List<Item> Items { get; private set; } = new();

    public int Count => Items.Count;

    public IReadOnlyList<Item> ReadOnlyItems => Items.AsReadOnly();

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
