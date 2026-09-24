using System;
using System.Collections.Generic;

namespace RPGGame;

public static class StoryTexts
{
    public static string Intro =>
        "Du bist ein Reisender in einer Welt, die von alten Geheimnissen und vergessenen Göttern durchdrungen ist. Die kleinen Dörfer am Rande des Waldes flüstern von seltsamen Lichtern in der Nacht und verlorenen Schätzen tief unter der Erde. Manche sagen, die Sterne selbst hätten einst über diese Lande gewacht, doch nun sind nur noch Ruinen und Geschichten geblieben.";

    public static string Town =>
        "Die Stadt Eldenruh ist ein sicherer Hafen für Abenteurer. Marktstände handeln mit Kräutern, Schmiede schleifen ihre Schwerter, und im Gasthaus werden Geschichten von Helden und Monstern erzählt. In einer abgelegenen Ecke des Marktes sitzt ein alter Kartenmacher, der behauptet, einen Hinweis auf eine vergessene Gruft zu besitzen.";

    public static string Dungeon =>
        "Tief unter den Hügeln liegt der Verließ von Morla. Kühle, feuchte Gänge führen zu Hallen, in denen Schatten leben. Jeder Schritt könnte ein Geheimnis oder eine Falle offenbaren. Alte Wandmalereien erzählen von einer Katastrophe, die die Priesterschaft einst beschworen hat — und von einem Schlüssel, der nie gefunden wurde.";

    public static string Victory =>
        "Nachdem die letzte Bestie gefallen ist, erhebt sich die Sonne über dem Horizont. Die Dunkelheit weicht, und du spürst, wie die Welt für einen Moment den Atem anhält. Dorfbewohner kommen aus ihren Häusern, um dir zu danken, und das Flüstern der Legenden beginnt erneut.";

    public static List<string> TownEvents { get; } = new()
    {
        "Ein Händler bietet seltene Kräuter im Tausch gegen eine Geschichte an.",
        "Im Gasthaus findet ein Liedermacher Trost für seine Sorgen in den Ohren der Reisenden.",
        "Eine Gruppe Söldner sucht Verstärkung für eine nächtliche Expedition in die Ruinen." 
    };

    public static List<string> DungeonEvents { get; } = new()
    {
        "Du findest ein zerbrochenes Siegel mit mystischen Zeichen.",
        "Eine versteckte Falltür gähnt in die Tiefe — etwas bewegt sich darunter.",
        "Die Luft riecht nach alten Kräutern; irgendwo flackert ein schwaches blaues Licht." 
    };

    public static Dictionary<string, string> MonsterDescriptions { get; } =
        new()
        {
            { "Goblin", "Kleine, verschlagene Kreaturen, die in Horden angreifen. Sie leben von Plünderung und Chaos, doch ein mutiger Schlag kann ihre Reihen zerreißen." },
            { "Skelett", "Die Überreste einstiger Krieger, gebunden an den Willen dunkler Magie. Ihre Knochen knacken leise, während sie auf ihre Beute zusteuern." },
            { "Drache", "Ein uraltes, mächtiges Wesen. Sein Feuer brennt wie ein Sturm und sein Zorn formt Legenden. Nur die Tapfersten wagen sich ihm entgegen." },
            { "Ork", "Breitschultrige Krieger mit rauer Zunge und noch rauerer Gewalttätigkeit. Ihre Stärke ist groß, doch ihr Ehrgefühl kann gegen sie verwendet werden." },
            { "Wraith", "Ein schattenhaftes Wesen, geboren aus Trauer und Verrat. Es saugt Wärme und Mut — nur Licht kann es verscheuchen." },
            { "Bandit", "Gewiefte Diebe, die Reisende auf isolierten Pfaden überfallen. Manchmal sind sie nur Diebe, manchmal Agenten dunkler Mächte." },
            { "Troll", "Langsame, mächtige Bestien mit zäher Haut. Ein stumpfer Hieb trifft sie hart, doch schnellere Gegner können ihre Schwächen finden." },
            { "Vampir", "Elegante, aber tödliche Jäger der Nacht. Sie nähren sich von Blut und verleihen den Tod mit einem Lächeln." },
            { "Riesenspinne", "Am Rand des Lichts lauert Webkunst und Geduld. Ihre Netze glänzen wie Tau und verbergen tödliche Fallen." }
        };

    public static Dictionary<string, string> NPCDialogues { get; } = new()
    {
        { "Wirtin", "Willkommen im Gasthaus. Setz dich, wärme dich am Feuer und höre den Geschichten. Wer weiß — vielleicht ist eine davon wahr." },
        { "Schmied", "Meine Klingen sind teurer als Truhe, doch sie schneiden durch das Dunkel. Zeig mir, was du hast, und ich werde sehen, ob ich es verbessern kann." },
        { "Kartenmacher", "Alte Karten lügen nie — sie verschweigen nur Dinge, die man nicht sehen will. Aber gegen eine Münze..." }
    };

    public static string GetMonsterDescription(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Ein unbekanntes Wesen blickt aus der Dunkelheit zurück.";
        }

        if (MonsterDescriptions.TryGetValue(name, out var desc))
        {
            return desc;
        }

        return $"{name}: Eine Kreatur mit einer eigenen Geschichte, die darauf wartet, entdeckt zu werden.";
    }

    public static string GetRandomTownEvent()
    {
        return TownEvents.Count == 0
            ? string.Empty
            : TownEvents[Random.Shared.Next(TownEvents.Count)];
    }

    public static string GetRandomDungeonEvent()
    {
        return DungeonEvents.Count == 0
            ? string.Empty
            : DungeonEvents[Random.Shared.Next(DungeonEvents.Count)];
    }

    public static string GetNPCDialogue(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;

        return NPCDialogues.TryGetValue(key, out var line) ? line : string.Empty;
    }

    // --- TOWN LOCATIONS, QUESTS & SHOPS ---

    // Quest-Template für Städte (Konvertierung zu Spiel-Quest erfolgt beim Übergeben)
    public record QuestInfo(
        string Title,
        string Description,
        string Giver,
        string TargetMonster,
        int RequiredKills,
        int RewardXP,
        int RewardGold,
        string RewardItemKey);

    public record ShopItem(string Key, string Name, string Description, int Price);

    public class TownLocation
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<QuestInfo> Quests { get; init; } = new();
        public List<ShopItem> ShopInventory { get; init; } = new();
        public string TavernDescription { get; init; } = string.Empty;
        public string TavernHealingText { get; init; } = string.Empty;
    }

    public static Dictionary<string, TownLocation> TownLocations { get; } =
        new()
        {
            {
                "Eldenruh",
                new TownLocation
                {
                    Name = "Eldenruh",
                    Description = "Eldenruh ist eine geschäftige Handelsstadt am Fluss. Händler aus fernen Ländern bieten exotische Waren, und die Stadtwache hält größtenteils Frieden.",
                    TavernDescription = "Die Taverne 'Zum Goldenen Krug' ist warm und laut; Reisende reden laut, und der Duft von Eintopf füllt die Luft.",
                    TavernHealingText = "Die Wirtin reicht dir einen Krug mit kräftiger Brühe und ein warmes Bett — deine Wunden schließen sich im Schlaf. (Gratis Heilung im Tausch gegen eine Geschichte.)",
                    Quests = new List<QuestInfo>
                    {
                        new QuestInfo(
                            "Die Plündererbande",
                            "Ein Trupp Banditen hat die Straße nach Norden unsicher gemacht. Sprich mit dem Hauptmann der Wache und finde ihre Lager.",
                            "Hauptmann Rolf",
                            "Bandit",
                            5,
                            80,
                            50,
                            ""),
                        new QuestInfo(
                            "Der verlorene Anhänger",
                            "Eine trauernde Frau hat ihren Familienanhänger verloren. Er könnte in den alten Ruinen westlich der Stadt liegen.",
                            "Marta die Weberin",
                            "",
                            0,
                            40,
                            30,
                            "SilverPendant"),
                        new QuestInfo(
                            "Die schwarze Krähe",
                            "Seltsame Krähen sammeln sich auf dem Marktplatz und stehlen kleine Gegenstände. Untersuche ihre Nester.",
                            "Wirtin",
                            "Riesenspinne",
                            2,
                            30,
                            15,
                            "")
                    },
                    ShopInventory = new List<ShopItem>
                    {
                        new ShopItem("HealthPotion", "Heiltrank", "Stellt eine moderate Menge Gesundheit wieder her.", 25),
                        new ShopItem("Antidote", "Gegenmittel", "Heilt Vergiftungen.", 15),
                        new ShopItem("IronSword", "Eisenschwert", "Eine einfache, robuste Waffe.", 75),
                        new ShopItem("LeatherArmor", "Lederrüstung", "Bietet leichten Schutz und Beweglichkeit.", 60)
                    }
                }
            }
        };

    public static TownLocation? GetTownLocation(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;

        TownLocations.TryGetValue(name, out var loc);
        return loc;
    }

    public static IEnumerable<QuestInfo> GetAvailableQuests(string townName)
    {
        var loc = GetTownLocation(townName);
        return loc == null ? Array.Empty<QuestInfo>() : loc.Quests;
    }

    public static IEnumerable<ShopItem> GetShopInventory(string townName)
    {
        var loc = GetTownLocation(townName);
        return loc == null ? Array.Empty<ShopItem>() : loc.ShopInventory;
    }

    public static string GetTavernHealing(string townName)
    {
        var loc = GetTownLocation(townName);
        return loc == null ? string.Empty : loc.TavernHealingText;
    }

    // Konvertiert Quest-Vorlagen der Stadt in echte Quest-Objekte und fügt sie dem Spieler hinzu.
    public static void AssignTownQuestsToPlayer(string townName, Player player)
    {
        if (player == null) return;

        var loc = GetTownLocation(townName);

        if (loc == null) return;

        foreach (QuestInfo q in loc.Quests)
        {
            // vorhandene Quest mit gleichem Namen nicht noch einmal hinzufügen
            bool exists = player.Quests.Exists(x => x.Name == q.Title);

            if (exists) continue;

            // Erstelle die Spiel-Quest (Program.Quest)
            Quest quest = new(
                q.Title,
                q.Description,
                q.TargetMonster ?? string.Empty,
                Math.Max(0, q.RequiredKills),
                Math.Max(0, q.RewardXP),
                Math.Max(0, q.RewardGold));

            player.AddQuest(quest);
        }
    }

    // Ruft die Gratis-Heilung der Taverne auf (setzt Leben und Mana zurück)
    public static int UseTavernHealing(Player player, string townName)
    {
        var loc = GetTownLocation(townName);

        if (loc == null || player == null) return 0;

        return player.RestAtTavern();
    }
}
