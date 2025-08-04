using Godot;
using System;

using Gmtk.Player;

namespace Gmtk.UI;
public partial class HealthBar : TextureProgressBar
{

    [Export] private Player.Player player;

    [Export] private Godot.Vector2 HEALTHBAR_SCALE = new(1, 1);
    [Export] private Godot.Vector2 HEALTHBAR_POSITION = new(10, 10);
    private const int HEALTH_TO_UI_SCALE = 2;
    private const int BAR_HEIGHT = 20;

    public override void _Ready()
    {
        if (player == null)
        {
            player = GetNode<Player.Player>("/root/World/Player");
        }

        this.Position = HEALTHBAR_POSITION;
        this.Scale = HEALTHBAR_SCALE;
        this.Size = new Vector2(HEALTH_TO_UI_SCALE * player.Health.MaxHealth, BAR_HEIGHT);
        this.Value = HEALTH_TO_UI_SCALE * player.Health.CurrHealth;

        player.Health.HealthChanged += PlayerHealthChanged;
    }

    private void PlayerHealthChanged(int new_health)
    {
        this.Value = new_health * this.MaxValue / player.Health.MaxHealth;
    }
}
