using Godot;

using Bookworm.Components;
using Bookworm.Weapon;

namespace Bookworm.Entity;

public partial class SidePlayer : CharacterBody2D
{

    // Components
    [Export] private HealthComponent health;
    [Export] private EnergyComponent energy;
    [Export] private HitboxComponent hitbox;
    [Export] private HurtboxComponent hurtbox;
    [Export] private AnimatedSprite2D sprite;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {

        Vector2 movement_input = new(Input.GetAxis("Left", "Right"), Input.GetAxis("Up", "Down"));

        HandleMove(delta);
        HandleJump(delta);
        HandleDash(delta);
        MoveAndSlide();
    }

    private void HandleMove(double delta)
    {
        Vector2 velocity = Velocity;

        Velocity = velocity;
    }

    private void HandleJump(double delta)
    {
    }

    private void HandleDash(double delta)
    {

    }



}
