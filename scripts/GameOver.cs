using Godot;
using System;

public partial class GameOver : CanvasLayer
{
    [Export] Button retryButton;

    LevelManager levelManager;


    public override void _Ready()
    {
        retryButton.Pressed += OnRetryButtonPressed;
        levelManager = GetNode<LevelManager>("/root/LevelManager");
    }

    private void OnRetryButtonPressed()
    {
        levelManager.RetryLevel();
    }
}