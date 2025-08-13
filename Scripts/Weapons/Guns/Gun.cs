using Godot;

using Bookworm.Utils;

namespace Bookworm.Weapon;
public partial class Gun : Node2D
{

    [Signal] public delegate void GunShotEventHandler(int num_bullets_used);
    [Signal] public delegate void GunReloadedEventHandler();

    [Export] protected string name = "Gun";
    [Export] protected int mag_size = 10;
    [Export] protected PackedScene bullet;
    private int curr_bullets;

    public int MagSize { get => mag_size; set => SetMagSize(value); }
    private void SetMagSize(int value)
    {
        mag_size = value;
        // Emit Signal
    }
    public int CurrBullets { get => curr_bullets; }

    // [Export] private int damage;
    [Export] private int time_between_shots = 200; // in time between bullet shots (in msec)
    [Export] private int fast_reload_time;
    [Export] private int normal_reload_time = 1200;
    [Export] private int reload_time_variance = 0;
    [Export] private int fast_reload_leniency;
    [Export] private float bullet_spread = 0f;
    [Export] private EntityUtils.EntityGroup damage_source = EntityUtils.EntityGroup.PLAYER;

    public EntityUtils.EntityGroup DamageSource { get => damage_source; set => damage_source = value; }
    public int ReloadTimeVariance { get => reload_time_variance; set => reload_time_variance = value; }


    // Timers
    private ulong bullet_shot_starttime;
    private ulong reload_starttime;

    // Booleans
    private bool bullet_shot = false;
    private bool is_reloading = false;

    // Components
    private AnimatedSprite2D sprite; // optional -- make UI for it later

    public override void _Ready()
    {
        curr_bullets = mag_size;
    }

    public override void _Process(double delta)
    {

    }

    private void HandleReload()
    {
        if (!is_reloading)
        {
            is_reloading = true;
            reload_starttime = Time.GetTicksMsec();
        }

        if (GameUtils.CheckTimerComplete(reload_starttime, normal_reload_time + reload_time_variance))
        {
            is_reloading = false;
            curr_bullets = mag_size;
            EmitSignal("GunReloaded");
        }
    }

    protected virtual void HandleShoot(Vector2 target_position)
    {
        ShootBullet(target_position);
        curr_bullets -= 1;
    }

    protected virtual void ShootBullet(Vector2 target_position)
    {
        bullet_shot_starttime = Time.GetTicksMsec();
        Bullet bullet_instance = bullet.Instantiate() as Bullet;
        GetTree().Root.AddChild(bullet_instance);
        bullet_instance.GlobalPosition = this.GlobalPosition;
        bullet_instance.SourceGroup = damage_source;
        bullet_instance.SetCollision(damage_source);
        bullet_instance.TargetDirection = -(this.GlobalPosition - target_position).Normalized();
    }

    public void ShootGun(Vector2 target_position)
    {
        if (this.curr_bullets <= 0)
        {
            HandleReload();
            return;
        }

        if (GameUtils.CheckTimerComplete(bullet_shot_starttime, time_between_shots))
        {
            HandleShoot(target_position);
        }

    }
}
