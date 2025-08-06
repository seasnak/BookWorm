using Godot;
using System;

using Bookworm.Components;
using Bookworm.Entity;
using Bookworm.Utils;

namespace Bookworm.Weapon;
public partial class Bullet : Node2D
{
    [Export] protected int speed = 100;
    public int Speed { get => speed; set => speed = value; }

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

        if (sprite == null)
        {
            sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        }
        // Random rand = new();
        // sprite.Frame = rand.Next(0, sprite.Hframes);

        uint PLAYER_HURTBOX_LAYER = 0b0100;
        uint ENEMY_HURTBOX_LAYER = 0b1000;

        if (source_group == EntityUtils.EntityGroup.ENEMY)
        {
            // hitbox.CollisionLayer = PLAYER_HURTBOX_LAYER;
            hitbox.SetCollisionLayer(PLAYER_HURTBOX_LAYER);
        }
        else
        {
            // hitbox.CollisionLayer = ENEMY_HURTBOX_LAYER;
            hitbox.SetCollisionLayer(ENEMY_HURTBOX_LAYER);
        }
        // hitbox.CollisionMask = 0b0;
        hitbox.SetCollisionMask(0b0);
        hitbox.Damage = damage;

        bullet_start_time = Time.GetTicksMsec();

        Random rand = new();
        rotation_speed += (float)rand.NextDouble() * 0.25f;

    }

    public override void _Process(double delta)
    {
        if (GameUtils.CheckTimerComplete(bullet_start_time, lifespan))
        {
            HandleDestroy();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMove(delta);
        HandleRotate(delta);
    }

    private void HandleDestroy()
    {
        hitbox.SetActive(false);
        hitbox.CollisionLayer = 0b0;
        hitbox.CollisionMask = 0b0;

        if (!destroy_animation_playing)
        {

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
        this.GlobalPosition += target_direction * (float)(delta * speed);
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

}


