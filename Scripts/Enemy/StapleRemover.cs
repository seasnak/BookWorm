using Godot;
using System;


namespace Bookworm.Entity;
public partial class StapleRemover : Enemy
{

    private Player player;

    public override void _Ready()
    {
        base._Ready();
        this.enemy_name = "Staple Remover";

        player = GetNode<Player>("/root/World/Player");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        this.sprite.FlipH = player.GlobalPosition.X < this.GlobalPosition.X;

    }

}
