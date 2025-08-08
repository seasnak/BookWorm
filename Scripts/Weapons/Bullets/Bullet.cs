using Godot;
using System;

using Bookworm.Components;
using Bookworm.Entity;
using Bookworm.Utils;

namespace Bookworm.Weapon;
public partial class Bullet : Node2D
{
    [Export] protected int movespeed = 100;
    public int Movespeed { get => movespeed; set => movespeed = value; }

    [Export] protected int lifespan = 3000;
    public int Lifespan { get => lifespan; set => lifespan = value; }

    [Export] protected int damage = 0;
    public int Damage { get => damage; set => damage = value; }

    [Export] protected float rotation_speed = 0.2f;
    public float RotationSpeed { get => rotation_speed; set => rotation_speed = value; }

    protected Vector2 target_direction;
    public Vector2 TargetDirection { get => target_direction; set => target_direction = value; }

    protected EntityUtils.EntityGroup source_group;
    public EntityUtils.EntityGroup SourceGroup { get => source_group; set => source_group = value; }

    // Timers
    private ulong bullet_start_time = 0;
    private ulong bullet_destroy_time = 0;

    // Components
    [Export] protected AnimatedSprite2D sprite;
    [Export] protected HitboxComponent hitbox;
    [Export] protected HurtboxComponent hurtbox;
    [Export] protected HealthComponent health;

    // Booleans
    private bool destroy_animation_playing = false;
    private bool destroy_animation_finished = false;

    public override void _Ready()
    {
        if (hitbox == null)
        {
            try
            {
                hitbox = GetNode<HitboxComponent>("HitboxComponent");
            }
            catch
            {
                GD.PrintErr("Could not find bullet hitbox");
            }
        }
        hitbox.AreaEntered += OnAreaEntered;

        if (hurtbox == null)
        {
            try
            {
                hurtbox = GetNode<HurtboxComponent>("HurtboxComponent");
            }
            catch
            {
                GD.PrintErr("Could not find bullet hurtbox");
            }
        }

        if (sprite == null)
        {
            sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        }
        // Random rand = new();
        // sprite.Frame = rand.Next(0, sprite.Hframes);

        uint PLAYER_HURTBOX_LAYER = 0b0100;
        uint ENEMY_HURTBOX_LAYER = 0b1000;
        uint BULLET_HURTBOX_LAYER = 0b10000;

        if (source_group == EntityUtils.EntityGroup.ENEMY)
        {
            hitbox.SetCollisionLayer(PLAYER_HURTBOX_LAYER);
            hurtbox.SetCollisionMask(BULLET_HURTBOX_LAYER);
        }
        else
        {
            hitbox.SetCollisionLayer(ENEMY_HURTBOX_LAYER);
        }
        hitbox.SetCollisionMask(0b0);
        hitbox.Damage = damage;
        hurtbox.SetCollisionLayer(0b0);

        bullet_start_time = Time.GetTicksMsec();

        Random rand = new();
        rotation_speed += (float)rand.NextDouble() * 0.25f;

        sprite.AnimationFinished += OnAnimationFinished;

    }

    public override void _Process(double delta)
    {
        if (GameUtils.CheckTimerComplete(bullet_start_time, lifespan))
        {
            HandleDestroy();
        }

        if (health.CurrHealth <= 0) { HandleDestroy(); }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMove(delta);
        HandleRotate(delta);
    }

    private void HandleDestroy()
    {
        this.movespeed = 0;
        hitbox.SetActive(false);
        hitbox.CollisionLayer = 0b0;
        hitbox.CollisionMask = 0b0;

        if (!destroy_animation_playing)
        {
            bullet_destroy_time = Time.GetTicksMsec();
            try
            {
                sprite.Play("destroy");
            }
            catch
            {
                GD.PrintErr($"No destroy animation for {this.Name}");
            }
        }

        if (destroy_animation_finished)
        {
            QueueFree();
        }
    }

    public void SetCollision(uint collision_layer, uint collision_mask = 0b0)
    {
        hitbox.CollisionLayer = collision_layer;
        hitbox.CollisionMask = collision_mask;

    }

    public void SetCollision(EntityUtils.EntityGroup entity_group)
    {
        uint PLAYER_HURTBOX_LAYER = 0b0100;
        uint ENEMY_HURTBOX_LAYER = 0b1000;

        if (entity_group == EntityUtils.EntityGroup.PLAYER) hitbox.SetCollisionMask(ENEMY_HURTBOX_LAYER);
        else hitbox.SetCollisionMask(PLAYER_HURTBOX_LAYER);
        // hitbox.CollisionMask = 0b0;
        hitbox.SetCollisionLayer(0b0);
    }

    protected void HandleMove(double delta)
    {
        this.GlobalPosition += target_direction * (float)(delta * movespeed);
    }

    protected void HandleRotate(double delta)
    {
        float rotation_amount = (float)(rotation_speed * delta);
        this.Rotate(rotation_amount);
    }

    protected virtual void OnAreaEntered(Node2D body)
    {
        if (body is not HurtboxComponent) return;
        if (body == null) return;

        if (source_group == EntityUtils.EntityGroup.ENEMY && body.Owner is Enemy) return;
        else if (source_group == EntityUtils.EntityGroup.PLAYER && body.Owner is Player) return;

        //TODO: Play Animation
        QueueFree();
    }

    private void OnAnimationFinished()
    {
        if (sprite.Animation == "destroy")
        {
            destroy_animation_finished = true;
        }
    }

}


