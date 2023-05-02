using Godot;
using System;

public partial class Attack : CharacterBody2D
{
    [Export] float projectileSpeed;
    [Export] public bool isBolt = false;

    [Export] Timer untilDestoryTimer;

    Vector2 velocity;
    Vector2 direction;
    Vector2 mousePos;
    Vector2 localPos;
    float controllerYPos;

    Player player;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/main/player");

        {
            direction = new Vector2(1, 0);
        }
    }

    public override void _Process(double delta)
    {
        velocity = direction * projectileSpeed;

        Velocity = velocity;
        MoveAndSlide();

        if (untilDestoryTimer.TimeLeft == 0)
        {
            QueueFree();
        }
    }
}
