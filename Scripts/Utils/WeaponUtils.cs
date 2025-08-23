using Godot;

namespace Bookworm.Weapon;
public partial class WeaponUtils : Node2D
{

    string WEAPONS_DATA_FILE = "res://Data/weapons.json";

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public Gun LoadGun(string name)
    {
        Gun gun = new();

        return gun;
    }

    public Gun LoadGunFromPrefab(string name)
    {
        Gun gun = new();

        return gun;
    }

}
