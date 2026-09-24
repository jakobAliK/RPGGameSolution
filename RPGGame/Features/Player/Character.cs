namespace RPGGame;

// ============================================================
// CHARACTER
// ============================================================

public class Character
{
    public string Name { get; internal set; } = "";

    public int MaxHealth { get; internal set; }
    public int Health { get; internal set; }

    public int Attack { get; internal set; }
    public int Defense { get; internal set; }

    public int DodgeChance { get; internal set; }
    public int CriticalChance { get; internal set; }

    public int PoisonTurns { get; internal set; }
    public int PoisonDamage { get; internal set; }

    public int BurnTurns { get; internal set; }
    public int BurnDamage { get; internal set; }

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
