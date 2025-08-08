using Godot;
using System;

using Bookworm.Entity;

namespace Bookworm.UI;
public partial class ShieldUI : TextureRect
{
    [Export] private Player player;
    [Export] private Texture2D can_shield_texture;
    [Export] private Texture2D cant_shield_texture;

    public override void _Ready()
    {
        player.CanShield += OnCanShieldChanged;

        this.Texture = can_shield_texture;
        this.Position = new(225, 40);
        this.Size = new(76, 32);

    }

    public override void _Process(double delta)
    {
    }

    private void OnCanShieldChanged(bool value)
    {
        this.Texture = (value == true) ? can_shield_texture : cant_shield_texture;
    }
}
