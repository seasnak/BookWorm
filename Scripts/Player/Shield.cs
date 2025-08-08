using Godot;
using System;

using Bookworm.Components;

namespace Bookworm.Entity;
public partial class Shield : Area2D
{
    // Stats
    [Export] private int energy_restored_on_hit = 20;
    public int EnergyRestoredOnHit { get => energy_restored_on_hit; }

    [Export] private int duration = 10;
    public int Duration { get => duration; }

    // Components
    [Export] private Player source;
    private Sprite2D sprite;
    private CollisionShape2D collider;

    // Timers
    private ulong activate_start_time = 0;

    public override void _Ready()
    {
        if (source == null)
        {
            source = GetParent<Player>();
        }

        if (sprite == null)
        {
            sprite = GetNode<Sprite2D>("Sprite2D");
        }

        if (collider == null)
        {
            collider = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        this.Visible = false;
        collider.Disabled = true;
        uint ENEMY_BULLET_COLLISION_MASK = 0b10000;
        this.SetCollisionMask(ENEMY_BULLET_COLLISION_MASK);
        source.ShieldActivate += OnShieldActivate;
    }

    public override void _Process(double delta)
    {

    }

    private void OnShieldActivate(int duration)
    {
        this.Visible = true;
        collider.Disabled = false;
    }



}
