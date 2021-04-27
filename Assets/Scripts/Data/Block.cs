using System;

public class Block
{
    public Action<bool> OnDeath;
    public int HealthMax { get; protected set; } = 10;
    public int Health { get; protected set; } = 10;
    public bool Dead { get; protected set; }

    public void TakeDamage(int amount, bool isPlayer) {
        Health -= amount;
        if(Health <= 0)
        {
            Die(isPlayer);
        }
    }

    public void Die(bool isPlayer)
    {
        if(Dead) { return; }
        Dead = true;
        OnDeath?.Invoke(isPlayer);
    }
}
