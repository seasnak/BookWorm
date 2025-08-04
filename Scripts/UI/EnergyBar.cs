using Godot;
using System;

using Gmtk.Player;

namespace Gmtk.UI;
public partial class EnergyBar : TextureProgressBar
{

    [Export] private Player.Player player;

    [Export] private Godot.Vector2 ENERGYBAR_SCALE = new(1, 1);
    [Export] private Godot.Vector2 ENERGYBAR_POSITION = new(10, 35);
    private const int ENERGY_TO_UI_SCALE = 2;
    private const int BAR_HEIGHT = 20;

    public override void _Ready()
    {
        if (player == null)
        {
            player = GetNode<Player.Player>("/root/World/Player");
        }

        this.Position = ENERGYBAR_POSITION;
        this.Scale = ENERGYBAR_SCALE;
        this.Size = new Vector2(ENERGY_TO_UI_SCALE * player.Energy.MaxEnergy, BAR_HEIGHT);
        this.Value = ENERGY_TO_UI_SCALE * player.Energy.CurrEnergy;

        player.Energy.EnergyChanged += PlayerEnergyChanged;
    }

    private void PlayerEnergyChanged(int new_energy)
    {
        this.Value = new_energy * this.MaxValue / player.Energy.MaxEnergy;
    }
}
