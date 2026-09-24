namespace RPGGame;

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
