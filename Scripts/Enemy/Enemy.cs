using System;
using Godot;

using Bookworm.Components;
using Bookworm.Weapon;
using Bookworm.Autoload;

namespace Bookworm.Entity;
public partial class Enemy : CharacterBody2D
{
    protected string enemy_name;

    public string EnemyName { get => enemy_name; set => enemy_name = value; }

    [Export] protected HealthComponent health;
    [Export] protected TextureProgressBar healthbar;
    [Export] protected HurtboxComponent hurtbox;
    [Export] protected HitboxComponent hitbox;
    [Export] protected CollisionShape2D collider;
    [Export] protected AnimatedSprite2D sprite;
    [Export] protected Gun gun;
    [Export] protected double fire_rate = 1;

    private double time_since_last_shot = 0;
    private ulong death_animation_starttime = 0;

    private bool is_dead = false;
    private bool death_animation_playing = false;

    public override void _Ready()
    {
        this.AddToGroup("Enemy");

        if (sprite == null)
        {
            sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        }

        if (hitbox == null)
        {
            hitbox = GetNode<HitboxComponent>("HitboxComponent");
        }

        if (hurtbox == null)
        {
            hurtbox = GetNode<HurtboxComponent>("HurtboxComponent");
        }

        if (collider == null)
        {
            collider = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        if (gun == null)
        {
            gun = GetNode<Gun>("Gun");
        }

        if (health == null)
        {
            health = GetNode<HealthComponent>("HealthComponent");
        }
        health.CurrHealth = health.MaxHealth;

        if (healthbar == null)
        {
            healthbar = GetNode<TextureProgressBar>("HealthBar");
        }


        uint PLAYER_HURTBOX_COLLISION_LAYER = 0b0100;
        uint ENEMY_HURTBOX_COLLISION_LAYER = 0b1000;

        hitbox.SetCollisionMask(PLAYER_HURTBOX_COLLISION_LAYER);
        hitbox.SetCollisionLayer(0b0);
        hurtbox.SetCollisionMask(0b0);
        hurtbox.SetCollisionLayer(ENEMY_HURTBOX_COLLISION_LAYER);

        hurtbox.HurtboxHit += OnEnemyHit;

        (sprite.Material as ShaderMaterial).SetShaderParameter("is_active", false);

        // randomize shooting interval
        time_since_last_shot -= (Double)GD.Randf();
    }

    public override void _Process(double delta)
    {
        if (is_dead) HandleDeath();
        healthbar.Value = health.CurrHealth;
        healthbar.MaxValue = health.MaxHealth;

    }

    public override void _PhysicsProcess(double delta)
    {
        if (time_since_last_shot + delta > fire_rate)
        {
            if (GetParent().GetNodeOrNull("Player") != null)
            {
                gun.ShootGun(GetNode<Player>("/root/World/Player").Position);
            }
            else
            {
                GD.Print("Could not shoot at player, does not exist");
            }
            time_since_last_shot = -(Double)GD.Randf();
        }
        else
        {
            time_since_last_shot += delta;
        }
    }

    private void HandleDeath()
    {
        hitbox.SetActive(false);
        hurtbox.SetActive(false);
        this.CollisionLayer = 0;
        this.CollisionLayer = 0;
        TempStats.num_enemies_killed += 1;

        if (!death_animation_playing)
        {
            sprite.Play("death");
            death_animation_playing = true;
            death_animation_starttime = Time.GetTicksMsec();
        }

        int DEATH_ANIMATION_DURATION = 200;
        if (Utils.Utils.CheckTimerComplete(death_animation_starttime, DEATH_ANIMATION_DURATION)) QueueFree();
    }

    public void Kill()
    {
        if (health.CurrHealth <= 0) is_dead = true;
    }

    private void OnEnemyHit()
    {
        AnimationPlayer hitflash_anim = GetNode<AnimationPlayer>("HitflashAnimationPlayer");
        hitflash_anim.CurrentAnimation = "hitflash";
        hitflash_anim.Play();
    }
}
