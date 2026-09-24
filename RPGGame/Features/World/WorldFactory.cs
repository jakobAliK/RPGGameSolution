namespace RPGGame;

// ============================================================
// WORLD / ORTE
// ============================================================

public static class WorldFactory
{
    public static Location CreateForest()
    {
        Location forest = new(
            "Verwunschener Wald",
            "Ein dichter Wald, voller Wilder Kreaturen und versteckter Schätze.",
            1,
            5);

        // Items
        forest.AddItem(new Item("Heiltrank (Groß)", "Heilt 75 HP.", ItemType.Verbrauchbar, 75, 75));
        forest.AddItem(new Item("Eichenholz", "Material vom Wald.", ItemType.Material, 5));

        // Monster
        forest.AddMonster(GameFactory.CreateGoblin());
        forest.AddMonster(GameFactory.CreateWolf());

        return forest;
    }

    public static Location CreateCave()
    {
        Location cave = new(
            "Dunkle Höhle",
            "Eine feuchte Höhle, in der stärkere Banditen und Monster hausen.",
            4,
            9);

        cave.AddItem(new Item("Eisenklinge", "Eine einfache Eisenklinge.", ItemType.Waffe, 120, 8));
        cave.AddItem(new Item("Brennpfeil", "Verursacht Brennen.", ItemType.Material, 30));

        cave.AddMonster(GameFactory.CreateBandit());
        cave.AddMonster(GameFactory.CreateSkeleton());

        return cave;
    }

    public static Location CreateTown()
    {
        Location town = new(
            "Eldenruh",
            "Ein sicherer Ort mit Händlern, Quests und Trainingsmöglichkeiten.",
            1,
            99);

        town.AddItem(new Item("Großer Heiltrank", "Heilt 150 HP.", ItemType.Verbrauchbar, 200, 150));
        town.AddItem(new Item("Lederschutz", "Geringe Rüstung.", ItemType.Rüstung, 80, 3));

        return town;
    }

    public static List<Location> GetAllLocations()
    {
        return new List<Location>
        {
            CreateForest(),
            CreateCave(),
            CreateTown()
        };
    }
}
