using Godot;
using System;

public partial class EndBg : Node2D
{
    [Export] Button exitButton;


    public override void _Ready()
    {
        exitButton.Pressed += OnExitButtonPressed;
    }


    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
