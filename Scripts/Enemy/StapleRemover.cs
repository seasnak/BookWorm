using Godot;
using System;

using Gmtk.Player;

namespace Gmtk.Enemy;
public partial class StapleRemover : Enemy
{

    private Player.Player player;

    public override void _Ready()
    {
        base._Ready();
        this.enemy_name = "Staple Remover";

        player = GetNode<Player.Player>("/root/World/Player");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        this.sprite.FlipH = player.GlobalPosition.X < this.GlobalPosition.X;

    }

}
