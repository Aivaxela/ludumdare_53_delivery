using Godot;
using System;

public partial class UserInterface : CanvasLayer
{
    [Export] Label carryingPackageLabel;
    [Export] Label dashCDLabel;
    [Export] Label levelEnemiesLabel;
    [Export] Label playerStateLabel;
    [Export] Label usingControllerLabel;

    [Export] AnimationPlayer animationPlayer;
    [Export] Label stunDurationLabel;
    [Export] Timer stunDurAnimChangeTimer;

    Player player;
    LevelManager levelManager;
    InputManager inputManager;


    public override void _Ready()
    {
        player = GetNode<Player>("/root/main/player");
        levelManager = GetNode<LevelManager>("/root/LevelManager");
        inputManager = GetNode<InputManager>("/root/InputManager");

        stunDurAnimChangeTimer.WaitTime = player.stunDuration;
    }

    public override void _Process(double delta)
    {
        if (player.hasPackage)
        {
            carryingPackageLabel.Text = "You have the package!";
        }
        else
        {
            carryingPackageLabel.Text = "You don't have the package!";
        }

        if (player.currentPlayerState == Player.State.STUNNED && stunDurAnimChangeTimer.TimeLeft == 0)
        {
            animationPlayer.Play("stun-duration-changed");
            stunDurAnimChangeTimer.WaitTime = player.stunDuration;
            stunDurAnimChangeTimer.Start();
        }

        stunDurationLabel.Text = Math.Round(player.stunDuration, 1).ToString();

        dashCDLabel.Text = "Dash Cooldown: " + player.dashCooldown;
        levelEnemiesLabel.Text = "Enemies left: " + levelManager.levelEnemiesLeft;
        playerStateLabel.Text = "Player State: " + player.currentPlayerState;
        usingControllerLabel.Text = "Using Controller: " + inputManager.UsingController();
    }
}
