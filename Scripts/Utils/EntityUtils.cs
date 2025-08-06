using Godot;

namespace Bookworm.Utils;
public partial class EntityUtils : Node
{

    public enum EntityGroup { ENEMY, PLAYER };

    public enum PlayerState
    {
        MOVING,
        DASHING,
        DRAWING,
        ATTACKING,
        KNOCKBACK,
        STUNNED,
    }

    public enum EnemyState
    {
        PASSIVE,
        ATTACKING,
        HIDING,
    }

    public const uint PLAYER_HURTBOX_COLLISION_LAYER = 0b0100;
    public const uint ENEMY_HURTBOX_COLLISION_LAYER = 0b1000;
}
