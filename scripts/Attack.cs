using Godot;
using System;

public partial class Attack : CharacterBody2D
{
    [Export] float projectileSpeed;

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
        // inputManager = GetNode<InputManager>("/root/InputManager");

        // if (!inputManager.UsingController())
        {
            mousePos = GetGlobalMousePosition();
            localPos = mousePos - player.GlobalPosition;
            direction = localPos.Normalized();
            Rotation = direction.Angle();
        }
        // else if (inputManager.UsingController())
        // {
        //     float horizontalInput = Input.GetJoyAxis(0, JoyAxis.LeftX);
        //     float verticalInput = Input.GetJoyAxis(0, JoyAxis.LeftY);

        //     direction = new Vector2(horizontalInput, verticalInput).Normalized();

        //     Rotation = direction.Angle();
        // }
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
