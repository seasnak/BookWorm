using Godot;

using Bookworm.Utils;
using Bookworm.Components;
using Bookworm.Entity;

namespace Bookworm.Object;
public partial class Pickup : Area2D
{
    [Export] private string pickup_name;
    public string PickupName { get => pickup_name; }

    // Components
    private AnimatedSprite2D sprite;
    private CollisionShape2D collider;

    // Booleans
    private bool destroy_animation_playing = false;
    private bool deestroy_animation_finished = false;

    public override void _Ready()
    {
        if (sprite == null) { sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); }
        if (collider == null) { collider = GetNode<CollisionShape2D>("CollisionShape2D"); }

        this.CollisionMask = EntityUtils.PLAYER_HURTBOX_COLLISION_LAYER;
        this.CollisionLayer = 0;
        this.AreaEntered += OnAreaEntered;

        sprite.AnimationFinished += OnAnimationFinished;
    }

    protected virtual void OnAreaEntered(Node2D other)
    {
        if (other is not HurtboxComponent) return;
        if (other.GetParent() is not Player) return;

        HandlePickedUp((Player)other);
    }

    protected virtual void OnAnimationFinished()
    {

    }

    protected virtual void HandlePickedUp(CharacterBody2D enteree)
    {

    }

    protected virtual void HandleDestroy()
    {
        QueueFree();
    }
}
