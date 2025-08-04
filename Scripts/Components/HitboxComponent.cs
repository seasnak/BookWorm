using Godot;
using System;

namespace Gmtk.Components;
public partial class HitboxComponent : Area2D
{

    private int damage = 1;
    public int Damage { get => damage; set => damage = value; }

    [Export] private string shape = "Circle";
    [Export] private Vector2 size;
    [Export] private float radius;

    private CollisionShape2D hitbox;

    public override void _Ready()
    {
        hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
        hitbox.Shape = new CircleShape2D();
        ((CircleShape2D)hitbox.Shape).Radius = radius;

        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {

    }

    public void SetActive(bool is_active = true)
    {
        hitbox.Disabled = !is_active;
    }

    private void OnAreaEntered(Node2D other)
    {
        if (other is not HurtboxComponent) return;
        if (other is null) return;
        if (other.Owner == this.Owner) return;

        ((HurtboxComponent)other).DamageHealth(damage);
    }

}
