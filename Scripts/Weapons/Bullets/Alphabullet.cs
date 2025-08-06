using Godot;
using System;

namespace Bookworm.Weapon;
public partial class Alphabullet : Bullet
{
    public override void _Ready()
    {
        base._Ready();



        Random random = new();
        sprite.Frame = random.Next(0, sprite.Hframes);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    protected override void OnAreaEntered(Node2D other)
    {
        base.OnAreaEntered(other);

    }


}
