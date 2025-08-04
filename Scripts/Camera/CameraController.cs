using Godot;
using System;

using Gmtk.Player;

namespace Gmtk.Camera;
public partial class CameraController : Camera2D
{

    [Export] private Node2D target;
    private const int MAX_DISTANCE_TO_TARGET = 8;
    private const int PANSPEED = 2;

    [Export] private Godot.Vector2 DEFAULT_ZOOM = new(4.5f, 4.5f);

    public override void _Ready()
    {
        this.AddToGroup("Enemy");

        if (target == null)
        {
            try
            {
                target = GetNode<CharacterBody2D>("/root/World/Player");
            }
            catch
            {
                GD.PrintErr("Camera could not find Player");
            }
        }

        this.Zoom = DEFAULT_ZOOM;
        this.Position = target.Position;
    }

    public override void _Process(double delta)
    {
        Vector2 curr_distance_to_target = target.GlobalPosition - this.GlobalPosition;

        if (curr_distance_to_target.Length() >= MAX_DISTANCE_TO_TARGET)
        {
            this.GlobalPosition += curr_distance_to_target.Normalized() * curr_distance_to_target.Length() * (float)(delta) * PANSPEED;
        }
    }

}
