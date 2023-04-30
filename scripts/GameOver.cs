using Godot;
using System;

public partial class GameOver : CanvasLayer
{
    [Export] Button retryButton;


    public override void _Ready()
    {
        retryButton.Pressed += OnRetryButtonPressed;
    }

    private void OnRetryButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/main.tscn");
    }
}