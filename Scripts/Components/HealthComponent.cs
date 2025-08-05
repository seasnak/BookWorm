using Godot;
using System;

namespace Bookworm.Components;

public partial class HealthComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler(int new_health);
    [Signal] public delegate void MaxHealthChangedEventHandler(int new_health);
    [Signal] public delegate void EntityDiedEventHandler();

    [Export] private int max_health = 100;
    private int curr_health;

    public int MaxHealth { get => max_health; set => max_health = value; }
    public int CurrHealth { get => curr_health; set => curr_health = value; }


    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void Damage(int value)
    {
        curr_health -= value;
        EmitSignal(SignalName.HealthChanged, curr_health);

        if (curr_health <= 0)
        {
            EmitSignal(SignalName.EntityDied);
        }
    }

    public void Heal(int value)
    {
        curr_health = Math.Min(max_health, curr_health + value);
        EmitSignal(SignalName.HealthChanged, curr_health);
    }

    public void SetCurrentHealth(int value)
    {
        curr_health = Math.Min(max_health, value);
        EmitSignal(SignalName.HealthChanged, curr_health);
    }

    public void SetMaxHealth(int value, bool fill_health = false)
    {
        max_health = value;
        if (fill_health)
        {
            curr_health = value;
            EmitSignal(SignalName.HealthChanged, curr_health);
        }
        EmitSignal(SignalName.MaxHealthChanged, max_health);
    }
}
