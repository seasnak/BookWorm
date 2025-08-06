using Godot;
using System;
using System.Collections.Generic;

using Bookworm.Components;
using Bookworm.Weapon;
using Bookworm.Autoload;

namespace Bookworm.Entity;
public partial class Player : CharacterBody2D
{
    // Stats
    private int movespeed = 75;
    private int movespeed_while_attacking = 65;
    private int movespeed_while_drawing = 125;
    private int dashspeed = 250;

    public int Movespeed { get => movespeed; set => movespeed = value; }
    public int Dashspeed { get => dashspeed; set => dashspeed = value; }

    // Timers
    private ulong dash_starttime;
    private int dash_duration = 130;
    private int dash_lockout = 500;

    private ulong draw_starttime;
    private int draw_duration_per_energy = 10;
    private int draw_duration;

    private int invuln_duration = 100;

    public int DashDuration { get => dash_duration; set => dash_duration = value; }
    public int DrawDuration { get => draw_duration; set => draw_duration = value; }
    public int InvulnDuration { get => invuln_duration; set => invuln_duration = value; }

    // Components
    [Export] private Line2D drawing_line;
    [Export] private HealthComponent health;
    [Export] private EnergyComponent energy;
    [Export] private HurtboxComponent hurtbox;
    [Export] private AnimatedSprite2D sprite;
    [Export] private Gun gun;

    public HealthComponent Health { get => health; }
    public EnergyComponent Energy { get => energy; }

    // Misc
    private List<Vector2> drawn_points = new();
    private const float LOOP_TOLERANCE_DISTANCE = 20f;
    private Vector2 targeter_location = new();

    // Booleans
    private bool is_dashing = false;
    private bool can_dash = true;
    private bool is_drawing = false;
    private bool checked_loop_for_enemies = false;
    private bool is_attacking = false;

    // Sprite Related Variables
    private EntityUtils.PlayerState current_action;
    private Vector2 movement_input;

    // Signals
    [Signal] public delegate void CanDashEventHandler(bool can_dash);

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
                // this.GetParent().AddChild(drawing_line);
                return;
            }
        }
        drawing_line.Width = 3.0f;
        drawing_line.DefaultColor = new(1f, 1f, 1f, 0.5f);
        draw_duration = energy.CurrEnergy * draw_duration_per_energy;

        if (hurtbox == null)
        {
            try { hurtbox = GetNode<HurtboxComponent>("HurtboxComponent"); }
            catch { GD.PrintErr("Could not find Player Hurtbox"); }
        }

        if (gun == null)
        {
            try { gun = GetNode<Gun>("Gun"); }
            catch { GD.PrintErr("Could not find Player Gun"); }
        }

        hurtbox.HurtboxHit += OnPlayerHit;
        uint PLAYER_HURTBOX_COLLISION_MASK = 0b0100;
        hurtbox.SetCollisionMask(0b0);
        hurtbox.SetCollisionLayer(PLAYER_HURTBOX_COLLISION_MASK);
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

    private void UpdateSprites()
    {
        if (Velocity.X == 0 && Velocity.Y == 0) return;
        else if (Velocity.X > 0)
        {
            sprite.Play("Right");
        }
        else if (Velocity.X < 0)
        {
            sprite.Play("Left");
        }
        else if (Velocity.Y > 0)
        {
            sprite.Play("Down");
        }
        else if (Velocity.Y < 0)
        {
            sprite.Play("Up");
        }
    }

    private void UpdateTargeterLocation()
    {
        if (GameSettings.CurrentInputMode == GameSettings.InputMode.KEYBOARD)
        {
            GD.Print("Here");
            targeter_location = GetGlobalMousePosition();
        }
        else
        {
            targeter_location = new(Input.GetAxis("AimLeft", "AimRight"), Input.GetAxis("AimDown", "AimUp"));
        }
    }

    private void HandleWin()
    {
        GetTree().ChangeSceneToFile("res://Scenes/winscreen.tscn");
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
            is_dashing = !CheckTimerComplete(dash_starttime, dash_duration);
            can_dash = false;
            EmitSignal("CanDash", false);
        }
        else if (!can_dash)
        {
            bool can_dash_temp = CheckTimerComplete(dash_starttime, dash_duration + dash_lockout);
            if (can_dash != can_dash_temp)
            {
                EmitSignal("CanDash", can_dash_temp);
                can_dash = can_dash_temp;
            }
        }

        if (is_drawing) { is_drawing = draw_duration > 0; }
    }


    private bool CheckTimerComplete(float timer_start_time, int duration)
    {
        if (Time.GetTicksMsec() - timer_start_time >= duration)
        {
            return true;
        }
        return false;
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateEnergy();

        movement_input = new(Input.GetAxis("Left", "Right"), Input.GetAxis("Up", "Down"));

        HandleMovement(movement_input);
        HandleDrawing(movement_input);
        HandleShoot(movement_input);

        MoveAndSlide();
    }

    private void HandleDeath()
    {
        GetTree().ChangeSceneToFile("res://Scenes/losescreen.tscn");
    }

    private void HandleMovement(Vector2 movement_input)
    {
        if (is_dashing)
        {
            // Velocity = Vector2.Zero;
            return;
        }

        Vector2 velocity = Velocity;

        if (can_dash && Input.IsActionJustPressed("Dash") && movement_input != Vector2.Zero)
        {
            is_dashing = true;
            dash_starttime = Time.GetTicksMsec();
            velocity = movement_input.Normalized() * dashspeed;
        }
        else
        {
            velocity = movespeed * movement_input;
        }
        Velocity = velocity;
    }

    private void HandleShoot(Vector2 movement_input)
    {
        if (Input.IsActionPressed("Shoot") && !is_drawing)
        {
            gun.ShootGun(targeter_location);
        }
    }

    private void HandleDrawing(Vector2 movement_input)
    {
        if (!is_drawing && Input.IsActionJustPressed("Draw") && energy.CurrEnergy > 0)
        {
            GD.Print("Player pressed \"Draw\"");
            drawn_points.Clear();
            drawing_line.ClearPoints();
            is_drawing = true;
            checked_loop_for_enemies = false;
        }
        else if (is_drawing && Input.IsActionJustPressed("Draw"))
        {
            GD.Print("Player stoppped drawing early");
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
        drawn_points.Add(position);
        drawing_line.AddPoint(position);
    }

    private void CheckForLoopAndKill()
    {
        int MINIMUM_VERTICE_COUNT = 3;
        GD.Print(drawn_points.Count);

        if (drawn_points.Count < MINIMUM_VERTICE_COUNT) return;

        Vector2 first_point = drawn_points[0];
        Vector2 last_point = drawn_points[^1];

        if (first_point.DistanceTo(last_point) <= LOOP_TOLERANCE_DISTANCE)
        {
            GD.Print("Loop Detected");
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
