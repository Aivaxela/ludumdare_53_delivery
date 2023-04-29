using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public enum State { FLYING, DASHING }
    public State currentPlayerState;

    [Export] float speed;
    [Export] float speedDelta;
    [Export] float dashSpeed;
    [Export] float inclineDrag;
    [Export] float declinePush;

    [Export] AnimationPlayer animationPlayer;
    [Export] Area2D packageArea;
    [Export] Area2D enemyArea;
    [Export] Timer recollectPackageTimer;
    [Export] Timer dashTimer;

    [Export] PackedScene attack;
    [Export] PackedScene package;

    public Vector2 velocity = Vector2.Zero;
    public Vector2 direction = Vector2.Zero;
    public bool hasPackage = true;
    public int hp = 100;

    Vector2 dashDirection;
    bool nearPackage = false;


    public override void _Ready()
    {
        currentPlayerState = State.FLYING;

        packageArea.BodyEntered += OnPackageAreaEntered;
        packageArea.BodyExited += OnPackageAreaExited;
        enemyArea.BodyEntered += OnEnemyAreaEntered;
    }

    public override void _Process(double delta)
    {
        direction.X = Input.GetActionStrength("move-right") - Input.GetActionStrength("move-left");

        switch (currentPlayerState)
        {
            case State.FLYING:
                _Process_Flying(delta);
                break;

            case State.DASHING:
                _Process_Dashing(delta);
                break;
        }

        Velocity = velocity;
        MoveAndSlide();

        if (Input.IsActionJustPressed("attack") && hasPackage == false)
        {
            CharacterBody2D newAttack = (CharacterBody2D)attack.Instantiate();
            GetParent().AddChild(newAttack);
            newAttack.GlobalPosition = GlobalPosition;
        }

        if (Input.IsActionJustPressed("toggle-package"))
        {
            if (hasPackage == false && nearPackage == true && recollectPackageTimer.TimeLeft == 0)
            {
                hasPackage = true;
                Package currentPackage = GetTree().GetFirstNodeInGroup("package") as Package;
                currentPackage.QueueFree();
            }

            else if (hasPackage == true)
            {
                hasPackage = false;
                CharacterBody2D newPackage = (CharacterBody2D)package.Instantiate();
                GetParent().AddChild(newPackage);
                newPackage.GlobalPosition = GlobalPosition;
                recollectPackageTimer.Start();
            }
        }

        if (Input.IsActionJustPressed("dash") && currentPlayerState != State.DASHING)
        {
            dashDirection = direction;
            animationPlayer.Play("dash");

            if (dashDirection != Vector2.Zero)
            {
                currentPlayerState = State.DASHING;
                dashTimer.Start();
            }
        }
    }

    public void _Process_Flying(double delta)
    {
        GetMovementVector();
        GetVelocity();
        SetMovementAnimations();
    }

    public void _Process_Dashing(double delta)
    {
        velocity = dashDirection * dashSpeed;

        if (dashTimer.TimeLeft == 0)
        {
            velocity = velocity / 2;
            currentPlayerState = State.FLYING;
        }
    }

    private void GetMovementVector()
    {
        direction.X = Input.GetActionStrength("move-right") - Input.GetActionStrength("move-left");
        direction.Y = Input.GetActionStrength("move-down") - Input.GetActionStrength("move-up");
    }

    private void GetVelocity()
    {
        if (direction.X != 0)
        {
            velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, speedDelta);
        }
        else
        {
            velocity.X = Mathf.Lerp(velocity.X, 0, speedDelta);
        }
        if (direction.Y < 0)
        {
            velocity.Y = Mathf.Lerp(velocity.Y, (direction.Y * speed) / inclineDrag, speedDelta);
        }
        if (direction.Y > 0)
        {
            velocity.Y = Mathf.Lerp(velocity.Y, (direction.Y * speed) * declinePush, speedDelta);
        }
        else
        {
            velocity.Y = Mathf.Lerp(velocity.Y, 0, speedDelta);
        }

        if (Mathf.Abs(direction.X) + Mathf.Abs(direction.Y) > 1)
        {
            velocity = velocity * 0.98f;
        }
    }

    private void SetMovementAnimations()
    {
        if (direction.Y < 0) //going up
        {
            if (hasPackage)
            {
                animationPlayer.Play("flying-up-package");
            }
            else
            {
                animationPlayer.Play("flying-up");
            }
        }
        else if (direction.Y > 0 && direction.X > 0) //going down and forward
        {
            if (hasPackage)
            {
                animationPlayer.Play("flying-down-forward-package");
            }
            else
            {
                animationPlayer.Play("flying-down-forward");
            }
        }
        else if (direction.Y > 0 && direction.X == 0) //going only down
        {
            if (hasPackage)
            {
                animationPlayer.Play("flying-down-package");
            }
            else
            {
                animationPlayer.Play("flying-down");
            }
        }
        else if (direction.Y > 0 || direction.X < 0) //going down or backward
        {
            if (hasPackage)
            {
                animationPlayer.Play("flying-down-backward-package");
            }
            else
            {
                animationPlayer.Play("flying-down-backward");
            }
        }
        else
        {
            if (hasPackage)
            {
                animationPlayer.Play("flying-idle-package");
            }
            else
            {
                animationPlayer.Play("flying-idle");
            }
        }
    }

    private void OnPackageAreaEntered(object _)
    {
        nearPackage = true;
        GD.Print("near package");
    }
    private void OnPackageAreaExited(object _)
    {
        nearPackage = false;
        GD.Print("not near package");
    }
    private void OnEnemyAreaEntered(object _)
    {
        hp -= 10;
    }
}