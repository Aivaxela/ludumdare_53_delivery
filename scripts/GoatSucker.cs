using Godot;
using System;

public partial class GoatSucker : CharacterBody2D
{
    [Export] float cruiseSpeed;
    [Export] float launchSpeed;

    [Export] Timer timeUntilLaunchTimer;
    [Export] Sprite2D sprite;
    [Export] Sprite2D wingSprite;
    [Export] AnimationPlayer animationPlayer;

    Vector2 velocity;
    Vector2 direction;

    bool directionLocked = false;


    public override void _Ready()
    {
        timeUntilLaunchTimer.WaitTime = GD.RandRange(3, 8);
        direction = new Vector2(-1, 0);
    }

    public override void _Process(double delta)
    {
        if (timeUntilLaunchTimer.TimeLeft == 0)
        {
            DirectTowardsPlayer();
            velocity = direction * launchSpeed;
        }
        else
        {
            velocity = direction * cruiseSpeed;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void DirectTowardsPlayer()
    {
        if (!directionLocked)
        {
            directionLocked = true;
            direction = (GetNode<Player>("/root/main/player").GlobalPosition - GlobalPosition).Normalized();
            Rotation = direction.Angle();
            sprite.FlipH = true;
            sprite.FlipV = true;
            wingSprite.FlipH = true;
            animationPlayer.Play("attacking");
        }
    }
}
