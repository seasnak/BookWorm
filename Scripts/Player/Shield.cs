using Godot;

using Bookworm.Weapon;
using Bookworm.Utils;

namespace Bookworm.Entity;
public partial class Shield : Area2D
{
    // Signals
    [Signal] public delegate void ShieldDurationChangedEventHandler(int new_duration);

    // Stats
    [Export] private int energy_restored_on_hit = 20;
    public int EnergyRestoredOnHit { get => energy_restored_on_hit; }


    // Components
    [Export] private Player source;
    private Sprite2D sprite;
    private CollisionShape2D collider;

    // Timers
    private ulong activate_start_time = 0;

    public override void _Ready()
    {
        if (source == null)
        {
            source = GetParent<Player>();
        }

        if (sprite == null)
        {
            sprite = GetNode<Sprite2D>("Sprite2D");
        }

        if (collider == null)
        {
            collider = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        SetShieldActive(false);
        source.ShieldActivate += OnShieldActivate;
    }

    public override void _Process(double delta)
    {

    }

    private void ToggleShieldActive()
    {
        SetShieldActive(!sprite.Visible);
    }

    private void OnShieldActivate(bool is_active)
    {
        SetShieldActive(is_active);
    }

    private void SetShieldActive(bool is_active)
    {
        sprite.Visible = is_active;
        this.CollisionLayer = is_active ? EntityUtils.PLAYER_HURTBOX_COLLISION_LAYER : 0b0;
    }

    public void HandleBulletCollision(Bullet bullet)
    {
        source.Energy.RestoreEnergy(energy_restored_on_hit);
    }
}
