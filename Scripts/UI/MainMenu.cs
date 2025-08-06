using Godot;
using System;

using Bookworm.Autoload;

namespace Bookworm.UI;
public partial class MainMenu : Node
{
    public override void _Ready()
    {
        Button button = GetNode<Button>("Button");
        button.ButtonUp += OnButtonUp;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Quit"))
        {
            GetTree().Quit();
        }
    }

    public void OnButtonUp()
    {
        GetTree().ChangeSceneToFile("res://Scenes/testscene.tscn");
    }
}
