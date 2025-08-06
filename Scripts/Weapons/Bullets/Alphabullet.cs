using Godot;
using System;

namespace Bookworm.Weapon;
public partial class Alphabullet : Bullet
{
    public override void _Ready()
    {
        base._Ready();

        Random random = new();
        int selected_letter = random.Next(0, sprite.SpriteFrames.GetFrameCount("default"));
        sprite.Stop();
        sprite.Frame = selected_letter;
        hitbox.Damage = (selected_letter + 1);
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
