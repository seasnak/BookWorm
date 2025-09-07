using Godot;
using System.Collections.Generic;

using Bookworm.Components;
using Bookworm.Weapon;
using Bookworm.Autoload;
using Bookworm.Utils;

namespace Bookworm.Entity;
public partial class Player : CharacterBody2D
{
    // Stats
    private int current_movespeed;
    private int movespeed = 120;
    private int movespeed_while_attacking = 95;
    private int movespeed_while_drawing = 150;
    private int dashspeed = 350;

    public int Movespeed { get => movespeed; set => movespeed = value; }
    public int Dashspeed { get => dashspeed; set => dashspeed = value; }

    // Timers
    private ulong dash_starttime;
    private int dash_duration = 120;
    private int dash_lockout = 500;

    private ulong draw_starttime;
    private int draw_duration_per_energy = 10;
    private int draw_duration;

    private ulong shield_starttime = 0;
    [Export] private int shield_duration = 1000;
    private int shield_health_cost = 0;
    private int shield_lockout = 4000;

    private int invuln_duration = 100;

    private int time_between_points = 100;
    private ulong last_point_added_time;

    public int DashDuration { get => dash_duration; set => dash_duration = value; }
    public int DrawDuration { get => draw_duration; set => draw_duration = value; }
    public int InvulnDuration { get => invuln_duration; set => invuln_duration = value; }

    // Components
    [Export] private Line2D drawing_line;
    private HealthComponent health;
    private EnergyComponent energy;
    private HurtboxComponent hurtbox;
    private AnimatedSprite2D sprite;
    [Export] private Gun gun;
    private Shield shield;

    public HealthComponent Health { get => health; }
    public EnergyComponent Energy { get => energy; }
    public AnimatedSprite2D Sprite { get => sprite; }
    public Gun Gun { get => gun; }
    public Shield Shield { get => shield; }

    // Misc
    private List<Vector2> drawn_points = new();
    private const float LOOP_TOLERANCE_DISTANCE = 20f;
    private Vector2 targeter_location = new();
    private int AIM_RETICLE_RADIUS = 40;
    [Export] private Sprite2D aim_reticle;

    // Booleans
    private bool is_dashing = false;
    private bool can_dash = true;
    private bool is_drawing = false;
    private bool checked_loop_for_enemies = false;
    private bool is_attacking = false;
    private bool is_shielding = false;
    private bool can_shield = true;

    // Sprite Related Variables
    private EntityUtils.PlayerState current_action;
    private Vector2 movement_input;

    // Signals
    [Signal] public delegate void CanDashEventHandler(bool can_dash);
    [Signal] public delegate void CanShieldEventHandler(bool can_shield);
    [Signal] public delegate void ShieldActivateEventHandler(bool is_active);

    public override void _Ready()
    {
        if (health == null)
        {
            try { health = GetNode<HealthComponent>("HealthComponent"); }
            catch { GD.PrintErr("Could not find Player HealthComponent"); }
        }
        health.SetCurrentHealth(health.MaxHealth);

        if (energy == null)
        {
            try { energy = GetNode<EnergyComponent>("EnergyComponent"); }
            catch { GD.PrintErr("Could not find Player EnergyComponent"); }
        }
        energy.SetCurrentEnergy(energy.MaxEnergy);

        if (sprite == null)
        {
            try { sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); }
            catch { GD.PrintErr("Could not find Player Sprite"); }
        }

        if (drawing_line == null)
        {
            try { drawing_line = GetNode<Line2D>("/root/World/DrawingLine"); }
            catch
            {
                GD.PrintErr("Could not find Node containing Drawing Line");
                return;
            }
        }
        drawing_line.ClearPoints();
        drawing_line.Width = 3.0f;
        drawing_line.DefaultColor = new(1f, 1f, 1f, 0.5f);
        draw_duration = energy.CurrEnergy * draw_duration_per_energy;

        if (hurtbox == null)
        {
            try { hurtbox = GetNode<HurtboxComponent>("HurtboxComponent"); }
            catch { GD.PrintErr("Could not find Player Hurtbox"); }
        }


        hurtbox.HurtboxHit += OnPlayerHit;
        hurtbox.SetCollisionMask(0b0);
        hurtbox.SetCollisionLayer(EntityUtils.PLAYER_HURTBOX_COLLISION_LAYER);

        if (gun == null)
        {
            try { gun = GetNode<Gun>("Gun"); }
            catch { GD.PrintErr("Could not find Player Gun"); }
        }

        if (aim_reticle == null)
        {
            try { aim_reticle = GetNode<Sprite2D>("/root/World/TargetReticle"); }
            catch { GD.PrintErr("Could not find Target Reticle"); }
        }
    }

    public override void _Process(double delta)
    {
        UpdateTimerChecks();

        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit();
        }

        if (health.CurrHealth <= 0) HandleDeath();

        UpdateTargeterLocation();
        UpdateSprites();

    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateEnergy();

        movement_input = new(Input.GetAxis("Left", "Right"), Input.GetAxis("Up", "Down"));

        HandleMovement(movement_input);
        HandleDrawing(movement_input);
        HandleShoot(movement_input);
        HandleShield();

        MoveAndSlide();
    }

    private void UpdateSprites()
    {
        if (Velocity.X == 0 && Velocity.Y == 0)
        {
            sprite.Play("idle");
            return;
        }
        else
        {
            sprite.Play("walk");
            sprite.FlipH = Velocity.X < 0;
        }
        // else if (Velocity.X > 0)
        // {
        //     sprite.Play("Right");
        // }
        // else if (Velocity.X < 0)
        // {
        //     sprite.Play("Left");
        // }
        // else if (Velocity.Y > 0)
        // {
        //     sprite.Play("Down");
        // }
        // else if (Velocity.Y < 0)
        // {
        //     sprite.Play("Up");
        // }
    }

    private void UpdateTargeterLocation()
    {
        if (GameSettings.CurrentInputMode == GameSettings.InputMode.KEYBOARD)
        {
            targeter_location = GetGlobalMousePosition();
        }
        else
        {
            targeter_location = this.GlobalPosition + new Vector2(Input.GetAxis("AimLeft", "AimRight"), Input.GetAxis("AimUp", "AimDown")) * AIM_RETICLE_RADIUS;
            aim_reticle.Visible = targeter_location != this.GlobalPosition;
        }
        aim_reticle.Position = targeter_location;
    }

    private void HandleWin()
    {
    }

    private void UpdateEnergy()
    {
        if (is_drawing)
        {
            draw_duration -= 1;
            if (draw_duration % draw_duration_per_energy == 0) energy.ExpendEnergy(1);
        }
    }

    private void UpdateTimerChecks()
    {
        if (is_dashing)
        {
            is_dashing = !Utils.GameUtils.CheckTimerComplete(dash_starttime, dash_duration);
            can_dash = false;
            EmitSignal("CanDash", false);
        }
        else if (!can_dash)
        {
            if (Utils.GameUtils.CheckTimerComplete(dash_starttime, dash_duration + dash_lockout))
            {
                EmitSignal("CanDash", true);
                can_dash = true;
            }
        }

        if (is_shielding)
        {
            is_shielding = !Utils.GameUtils.CheckTimerComplete(shield_starttime, shield_duration);
            if (!is_shielding)
            {
                EmitSignal("ShieldActivate", false);
            }
        }
        else if (!can_shield)
        {
            if (Utils.GameUtils.CheckTimerComplete(shield_starttime, shield_duration + shield_lockout))
            {
                EmitSignal("CanShield", true);
                can_shield = true;
            }
        }

        if (is_drawing) { is_drawing = draw_duration > 0; }
    }

    private void HandleDeath()
    {
    }

    private void HandleMovement(Vector2 movement_input)
    {
        if (is_dashing)
        {
            this.hurtbox.SetActive(false);
            return;
        }
        else
        {
            this.hurtbox.SetActive(true);
        }

        Vector2 velocity = Velocity;

        if (can_dash && Input.IsActionJustPressed("Dash") && movement_input != Vector2.Zero)
        {
            is_dashing = true;
            dash_starttime = Time.GetTicksMsec();
            velocity = movement_input.Normalized() * dashspeed;
        }
        else if (is_attacking)
        {
            velocity = movespeed_while_attacking * movement_input;
        }
        else if (is_drawing)
        {
            velocity = movespeed_while_drawing * movement_input;
        }
        else
        {
            velocity = movespeed * movement_input;
        }
        Velocity = velocity;
    }

    private void HandleShoot(Vector2 movement_input)
    {
        if (targeter_location == this.GlobalPosition) return;
        if (Input.IsActionPressed("Shoot") && !is_drawing)
        {
            gun.ShootGun(targeter_location);
        }
    }

    private void HandleShield()
    {
        if (is_shielding) return;

        if (can_shield && Input.IsActionPressed("Shield"))
        {
            EmitSignal("ShieldActivate", true);
            EmitSignal("CanShield", false);
            is_shielding = true;
            can_shield = false;
            shield_starttime = Time.GetTicksMsec();

            health.Damage(shield_health_cost);
        }
    }

    private void HandleDrawing(Vector2 movement_input)
    {
        if (!is_drawing && Input.IsActionJustPressed("Draw") && energy.CurrEnergy > 0)
        {
            // GD.Print("Player pressed \"Draw\"");
            drawn_points.Clear();
            drawing_line.ClearPoints();
            is_drawing = true;
            checked_loop_for_enemies = false;
        }
        else if (is_drawing && Input.IsActionJustPressed("Draw"))
        {
            // GD.Print("Player stoppped drawing early");
            is_drawing = false;
            CheckForLoopAndKill();
            checked_loop_for_enemies = true;
        }

        if (!is_drawing && drawn_points.Count >= 3)
        {
            if (!checked_loop_for_enemies)
            {
                checked_loop_for_enemies = true;
                CheckForLoopAndKill();
            }
            return;
        }

        AddPointToLine(this.Position);
        CheckForLoop(this.Position);
    }

    private void CheckForLoop(Vector2 position)
    {
        const int MIN_NUM_VERTICES = 50;
        if (drawn_points.Count < MIN_NUM_VERTICES) return;
        for (int i = 0; i < drawn_points.Count - MIN_NUM_VERTICES; i += 1)
        {
            if (drawn_points[i].DistanceTo(position) <= 3)
            {
                is_drawing = false;
                KillEnemiesInLoop(i, drawn_points.Count - 1);
                return;
            }
        }

        return;
    }

    private void AddPointToLine(Vector2 position)
    {
        if (drawn_points.Count > 3 && position.DistanceTo(drawn_points[^1]) < 3) return;
        drawn_points.Add(position);
        drawing_line.AddPoint(position);
    }

    private void CheckForLoopAndKill()
    {
        int MINIMUM_VERTICE_COUNT = 3;
        // GD.Print(drawn_points.Count);

        if (drawn_points.Count < MINIMUM_VERTICE_COUNT) return;

        Vector2 first_point = drawn_points[0];
        Vector2 last_point = drawn_points[^1];

        if (first_point.DistanceTo(last_point) <= LOOP_TOLERANCE_DISTANCE)
        {
            // GD.Print("Loop Detected");
            KillEnemiesInLoop();
        }
    }

    private void KillEnemiesInLoop()
    {
        KillEnemiesInLoop(0, drawn_points.Count - 1);
    }
    public void KillEnemiesInLoop(int start, int end)
    {
        var enemies = GetTree().GetNodesInGroup("Enemy");

        foreach (Node2D enemy in enemies)
        {
            if (Geometry2D.IsPointInPolygon(enemy.GlobalPosition, drawn_points.GetRange(start, end - start).ToArray()))
            {
                (enemy as Enemy)?.Kill();
            }
        }
    }

    private void OnPlayerHit() // Handle Invulnerability Frames
    {
        // hurtbox.SetActive(false);
    }

    private void OnEnemyHitWithBullet(int heal_amount)
    {
        energy.RestoreEnergy(heal_amount);
    }

}
