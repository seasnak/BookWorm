using Godot;

namespace Bookworm.Entity;
public partial class Rockfish : Enemy
{
    protected Player player;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        gun.ShootGun(this.Position + (Vector2.Up * 10));
    }

}
