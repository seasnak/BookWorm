using Godot;

using Bookworm.Utils;
using Bookworm.Components;
using Bookworm.Entity;

namespace Bookworm.Object;
public partial class Pickup : Area2D
{

    [Export] private string pickup_name;
    public string PickupName { get => pickup_name; }

    private Sprite2D sprite;
    private CollisionShape2D collider;

    public override void _Ready()
    {
        if (sprite == null) { sprite = GetNode<Sprite2D>("Sprite2D"); }
        if (collider == null) { collider = GetNode<CollisionShape2D>("CollisionShape2D"); }

        this.CollisionMask = EntityUtils.PLAYER_HURTBOX_COLLISION_LAYER;
        this.CollisionLayer = 0;
        this.AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Node2D other)
    {
        if (other is not HurtboxComponent) return;
    }

}
