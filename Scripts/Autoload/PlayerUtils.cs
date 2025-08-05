using Godot;
using System;

namespace Bookworm.Entity;
public partial class PlayerUtils : Node
{
    public enum PlayerState
    {
        MOVING,
        DASHING,
        DRAWING,
        SHOOTING,
    }

    public enum EnemyState
    {
        PASSIVE,
        AGGRO,
    }
}
