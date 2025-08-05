using Godot;
using System;

namespace Bookworm.Components;
public partial class HurtboxComponent : Area2D
{
    [Signal] public delegate void HurtboxHitEventHandler();

    [Export] private Shape2D hurtbox_shape;

    private AnimatedSprite2D sprite;
    private HealthComponent health;

    private CollisionShape2D hurtbox;

    public override void _Ready()
    {
        if (health == null)
        {
            health = GetParent().GetNode<HealthComponent>("HealthComponent");
        }

        hurtbox = GetNode<CollisionShape2D>("CollisionShape2D");
        if (hurtbox_shape != null) hurtbox.Shape = hurtbox_shape;
    }

    public override void _Process(double delta)
    {
    }

    public void DamageHealth(int amount)
    {
        health.Damage(amount);
        EmitSignal("HurtboxHit");
    }

    public void SetActive(bool is_active)
    {
        this.hurtbox.Disabled = !is_active;
    }

}
