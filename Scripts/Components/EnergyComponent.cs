using Godot;
using System;

namespace Bookworm.Components;

public partial class EnergyComponent : Node
{

    [Signal] public delegate void EnergyChangedEventHandler(int new_energy);
    [Signal] public delegate void MaxEnergyChangedEventHandler(int new_energy);

    [Export] private int max_energy = 100;
    private int curr_energy;

    public int MaxEnergy { get => max_energy; }
    public int CurrEnergy { get => curr_energy; }

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {

    }

    public void ExpendEnergy(int value)
    {
        curr_energy -= value;
        EmitSignal(SignalName.EnergyChanged, curr_energy);
    }

    public void RestoreEnergy(int value)
    {
        curr_energy = Math.Min(max_energy, curr_energy + value);
        EmitSignal(SignalName.EnergyChanged, curr_energy);
    }

    public void SetCurrentEnergy(int value)
    {
        curr_energy = Math.Min(max_energy, value);
        EmitSignal(SignalName.EnergyChanged, curr_energy);
    }

    public void SetMaxEnergy(int value, bool fill_energy = false)
    {
        max_energy = value;
        if (fill_energy)
        {
            curr_energy = value;
            EmitSignal(SignalName.EnergyChanged, curr_energy);
        }
        EmitSignal(SignalName.MaxEnergyChanged, max_energy);
    }
}
