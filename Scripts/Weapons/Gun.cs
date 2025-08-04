using Godot;
using System;

using Gmtk.Utils;

namespace Gmtk.Weapon;

public partial class Gun : Node2D
{

    [Signal] public delegate void GunShotEventHandler(int num_bullets_used);
    [Signal] public delegate void GunReloadedEventHandler();

    [Export] private string name = "Gun";
    [Export] private int mag_size = 10;
    [Export] private PackedScene bullet;
    private int curr_bullets;

    public int MagSize { get => mag_size; }
    public int CurrBullets { get => curr_bullets; }

    // [Export] private int damage;
    [Export] private int time_between_shots = 200; // in time between bullet shots (in msec)
    [Export] private int fast_reload_time;
    [Export] private int normal_reload_time = 1200;
    [Export] private int fast_reload_leniency;
    [Export] private Utils.Utils.EntityGroups damage_source = Utils.Utils.EntityGroups.PLAYER;

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

        if (Utils.Utils.CheckTimerComplete(reload_starttime, normal_reload_time))
        {
            is_reloading = false;
            curr_bullets = mag_size;
        }
    }

    protected void HandleShoot(Vector2 target_position)
    {
        bullet_shot_starttime = Time.GetTicksMsec();
        Bullet bullet_instance = bullet.Instantiate() as Bullet;
        GetTree().Root.AddChild(bullet_instance);
        bullet_instance.GlobalPosition = this.GlobalPosition;
        bullet_instance.SourceGroup = damage_source;
        bullet_instance.SetCollision(damage_source);
        bullet_instance.TargetDirection = -(this.GlobalPosition - target_position).Normalized();
        curr_bullets -= 1;
    }

    public void ShootGun(Vector2 target_position)
    {
        if (this.curr_bullets <= 0)
        {
            HandleReload();
            return;
        }

        if (Utils.Utils.CheckTimerComplete(bullet_shot_starttime, time_between_shots))
        {
            HandleShoot(target_position);
        }

    }
}
