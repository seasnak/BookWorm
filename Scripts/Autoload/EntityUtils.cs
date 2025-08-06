using Godot;

namespace Bookworm.Entity;
public partial class EntityUtils : Node
{
    public enum PlayerState
    {
        MOVING,
        DASHING,
        DRAWING,
        ATTACKING,
    }

    public enum EnemyState
    {
        PASSIVE,
        AGGRO,
    }

    public const uint PLAYER_HURTBOX_COLLISION_LAYER = 0b0100;
    public const uint ENEMY_HURTBOX_COLLISION_LAYER = 0b1000;
}
