using Godot;
using System;

public partial class UserInterface : CanvasLayer
{
    [Export] AnimationPlayer animationPlayer;
    [Export] Label stunDurationLabel;
    [Export] Label dashCdLabel;
    [Export] Label boltCdLabel;
    [Export] Label enemiesRemainingLabel;
    [Export] Timer stunDurAnimChangeTimer;
    [Export] Sprite2D dashReadySprite;
    [Export] Sprite2D boltReadySprite;

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
        if (player.currentPlayerState == Player.State.STUNNED && stunDurAnimChangeTimer.TimeLeft == 0)
        {
            animationPlayer.Play("stun-duration-changed");
            stunDurAnimChangeTimer.WaitTime = player.stunDuration;
            stunDurAnimChangeTimer.Start();
        }

        if (player.dashCooldownTimer.TimeLeft == 0) { dashReadySprite.Visible = true; dashCdLabel.Visible = false; }
        else { dashReadySprite.Visible = false; dashCdLabel.Visible = true; }
        if (player.attackL1CooldownTimer.TimeLeft == 0) { boltReadySprite.Visible = true; boltCdLabel.Visible = false; }
        else { boltReadySprite.Visible = false; boltCdLabel.Visible = true; }

        stunDurationLabel.Text = Math.Round(player.stunDuration, 1).ToString();
        dashCdLabel.Text = Math.Round((float)player.dashCooldown, 1).ToString();
        boltCdLabel.Text = Math.Round((float)player.boltCd, 1).ToString();
        enemiesRemainingLabel.Text = levelManager.levelEnemiesLeft.ToString();
    }
}
