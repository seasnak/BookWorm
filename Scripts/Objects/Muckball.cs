using Godot;

using Bookworm.Components;
using Bookworm.Entity;

namespace Bookworm.Object;
public partial class Muckball : Pickup
{

    private int ENERGY_RESTORE_AMOUNT = 20;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {

    }

    protected override void HandlePickedUp(CharacterBody2D enteree)
    {
        ((Player)enteree).Energy.RestoreEnergy(ENERGY_RESTORE_AMOUNT);
    }

    protected override void HandleDestroy()
    {
        QueueFree();
    }
}
