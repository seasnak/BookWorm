using Godot;
using System;

namespace Bookworm.Weapon;
public partial class BubbleGun : Gun
{

    [Export] private int NUM_BULLETS_BURST_FIRED = 3;
    [Export] private int BULLET_SPREAD_ANGLE = 10;

    public override void _Ready()
    {
        base._Ready();

        this.name = "Bubble Gun";
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    protected override void HandleShoot(Vector2 target_position)
    {
        Random rand = new();
        Vector2 angle_of_variance = new(
                (float)(rand.NextDouble() - 0.5f) * BULLET_SPREAD_ANGLE * 2,
                (float)(rand.NextDouble() - 0.5f) * BULLET_SPREAD_ANGLE * 2
        );
        for (int i = 0; i < NUM_BULLETS_BURST_FIRED; i++)
        {
            ShootBullet(target_position + angle_of_variance - (new Vector2(i, i) * 2));
        }
    }

}
