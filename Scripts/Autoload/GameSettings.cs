using Godot;

namespace Bookworm.Autoload;
public partial class GameSettings : Node
{


    public enum InputMode
    {
        KEYBOARD,
        CONTROLLER,
    }

    private static InputMode current_input_mode = InputMode.KEYBOARD;
    public static InputMode CurrentInputMode { get => current_input_mode; }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventJoypadMotion && current_input_mode != InputMode.CONTROLLER)
        {
            GD.Print("Input Mode Changed to Controller");
            current_input_mode = InputMode.CONTROLLER;
        }

        if (@event is InputEventMouseMotion && current_input_mode != InputMode.KEYBOARD)
        {
            GD.Print("Input Mode Changed to Keyboard");
            current_input_mode = InputMode.KEYBOARD;

        }
    }

}
