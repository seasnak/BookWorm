using Godot;
using System;

using Bookworm.Entity;

namespace Bookworm.UI;
public partial class DashUI : TextureRect
{
    [Export] private Player player;
    [Export] private Texture2D dash_available_texture;
    [Export] private Texture2D dash_on_cooldown_texture;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/World/Player");
        player.CanDash += OnCanDashChanged;

        this.Texture = dash_available_texture;
        this.Position = new(225, 10);
        this.Size = new(64, 32);
    }

    public override void _Process(double delta)
    {

    }

    private void OnCanDashChanged(bool new_value)
    {
        if (new_value == true)
        {
            this.Texture = dash_available_texture;
        }
        else
        {
            this.Texture = dash_on_cooldown_texture;
        }
    }

}
