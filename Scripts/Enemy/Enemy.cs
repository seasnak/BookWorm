using System;
using Godot;

using Bookworm.Components;
using Bookworm.Weapon;
using Bookworm.Utils;

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

    private ulong death_animation_starttime = 0;

    private bool is_dead = false;
    private bool is_damaged = false;
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
        gun.DamageSource = EntityUtils.EntityGroup.ENEMY;

        if (health == null)
        {
            health = GetNode<HealthComponent>("HealthComponent");
        }
        health.CurrHealth = health.MaxHealth;

        if (healthbar == null)
        {
            healthbar = GetNode<TextureProgressBar>("EntityLocalUI/HealthBar");
        }

        hitbox.SetCollisionMask(EntityUtils.PLAYER_HURTBOX_COLLISION_LAYER);
        hitbox.SetCollisionLayer(0b0);
        hurtbox.SetCollisionMask(0b0);
        hurtbox.SetCollisionLayer(EntityUtils.ENEMY_HURTBOX_COLLISION_LAYER);

        hurtbox.HurtboxHit += OnEnemyHit;
        health.HealthChanged += OnHealthChanged;

        (sprite.Material as ShaderMaterial).SetShaderParameter("is_active", false);
        gun.GunReloaded += OnGunReloaded;
    }

    public override void _Process(double delta)
    {
        if (is_dead) HandleDeath();
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    private void HandleDeath()
    {
        gun.MagSize = 0;
        hitbox.SetCollisionMask(0);
        hurtbox.SetCollisionLayer(0);
        this.CollisionLayer = 0;
        this.CollisionMask = 0;

        if (!death_animation_playing)
        {
            sprite.Play("death");
            death_animation_playing = true;
            death_animation_starttime = Time.GetTicksMsec();
        }

        int DEATH_ANIMATION_DURATION = 1000;
        if (GameUtils.CheckTimerComplete(death_animation_starttime, DEATH_ANIMATION_DURATION)) QueueFree();
    }

    private void UpdateHealthbar()
    {
        healthbar.Value = health.CurrHealth;
        healthbar.MaxValue = health.MaxHealth;
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

    private void OnHealthChanged(int new_value)
    {
        if (health.CurrHealth <= 0)
        {
            is_damaged = true;
            sprite.Stop();
            int curr_frame = sprite.Frame;
            sprite.Play("damaged");
            sprite.Stop();
            sprite.Frame = curr_frame;
        }

        UpdateHealthbar();
    }

    private void OnGunReloaded()
    {
        gun.ReloadTimeVariance = (int)(GD.Randf() * 60);
    }
}
