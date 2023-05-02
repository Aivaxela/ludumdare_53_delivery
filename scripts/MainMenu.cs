using Godot;
using System;

public partial class MainMenu : Node
{
    [Export] Button startButton;
    [Export] Button exitButton;


    public override void _Ready()
    {
        startButton.Pressed += OnStartButtonPressed;
        exitButton.Pressed += OnExitButtonPressed;
    }


    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/intro-level.tscn");
    }
    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
