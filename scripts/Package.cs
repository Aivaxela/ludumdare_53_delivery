using Godot;
using System;

public partial class Package : CharacterBody2D
{
    [Export] float speed;
    [Export] float gravity;

    [Export] Timer checkWallCollisionTimer;

    Vector2 velocity;
    Vector2 direction;
    float currentXPos1;
    float currentXPos2;
    double randX;
    bool stopYVelocityGain = false;
    bool checkPos1 = true;
    bool bounced = false;


    public override void _Ready()
    {
        randX = GD.RandRange(-0.65, 1.35);
        direction = new Vector2((float)randX, -1);

        velocity.X = speed;
        velocity = direction * speed;
    }

    public override void _Process(double delta)
    {
        velocity.X = Mathf.Lerp(velocity.X, 0, (float)delta * 0.8f);

        if (!stopYVelocityGain)
        {
            velocity.Y += gravity * (float)delta;
        }

        if (checkWallCollisionTimer.TimeLeft == 0 && checkPos1)
        {
            currentXPos1 = GlobalPosition.X;
            checkPos1 = false;
        }
        else
        {
            currentXPos2 = GlobalPosition.X;
            checkPos1 = true;
        }
        if (Mathf.Abs(currentXPos1) - Mathf.Abs(currentXPos2) == 0 && !bounced)
        {
            bounced = true;
            velocity.X = (float)-randX * speed;
        }

        GD.Print(velocity.X);


        Velocity = velocity;
        MoveAndSlide();

        if (velocity.Y > 100)
        {
            stopYVelocityGain = true;
        }
    }
}
