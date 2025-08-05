using Godot;
using System;

namespace Bookworm.Entity;
public partial class PlayerUtils : Node
{
    public enum PlayerActionState
    {
        MOVING,
        DASHING,
        DRAWING,
        SHOOTING,
    }
}
