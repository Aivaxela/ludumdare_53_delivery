using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public enum State { FLYING, DASHING, STUNNED }
    public State currentPlayerState;

    [Export] float speed;
    [Export] float speedDelta;
    [Export] float dashSpeed;
    [Export] float declinePush;

    [Export] AnimationPlayer animationPlayer;
    [Export] Area2D packageArea;
    [Export] Area2D enemyArea;
    [Export] Timer recollectPackageTimer;
    [Export] Timer dashTimer;
    [Export] Timer dashCooldownTimer;
    [Export] Timer attackCooldownTimer;
    [Export] Timer stunnedTimer;
    [Export] PackedScene attack;
    [Export] PackedScene package;
    [Export] Sprite2D spriteBody;

    [Export] public int dashCooldown;

    public Vector2 velocity = Vector2.Zero;
    public Vector2 direction = Vector2.Zero;
    public bool hasPackage = true;

    Vector2 dashDirection;
    Vector2 mouseVector;
    bool nearPackage = false;
    bool stunned = false;


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
        dashCooldown = (int)dashCooldownTimer.TimeLeft;

        switch (currentPlayerState)
        {
            case State.FLYING:
                _Process_Flying(delta);
                break;

            case State.DASHING:
                _Process_Dashing(delta);
                break;
            case State.STUNNED:
                _Process_Stunned(delta);
                break;
        }

        Velocity = velocity;
        MoveAndSlide();

        if (Input.IsActionPressed("attack") && hasPackage == false && attackCooldownTimer.TimeLeft == 0)
        {
            CharacterBody2D newAttack = (CharacterBody2D)attack.Instantiate();
            GetParent().AddChild(newAttack);
            newAttack.GlobalPosition = GlobalPosition;
            attackCooldownTimer.Start();
        }

        if (Input.IsActionJustPressed("toggle-package"))
        {
            if (hasPackage == false && nearPackage == true && recollectPackageTimer.TimeLeft == 0 && !stunned)
            {
                hasPackage = true;
                Package currentPackage = GetTree().GetFirstNodeInGroup("package") as Package;
                currentPackage.QueueFree();
            }

            else if (hasPackage == true)
            {
                DropPackage();
            }
        }

        if (Input.IsActionJustPressed("dash") && currentPlayerState != State.DASHING && hasPackage == false && dashCooldownTimer.TimeLeft == 0)
        {
            dashDirection = GetMousePosAgainstPlayerPosVector();

            if (dashDirection != Vector2.Zero)
            {
                dashTimer.Start();
                dashCooldownTimer.Start();
                Rotation = dashDirection.Angle();
                currentPlayerState = State.DASHING;
            }
        }
    }

    public void _Process_Dashing(double delta)
    {
        animationPlayer.Play("dashing");
        velocity = dashDirection * dashSpeed;
        if (velocity.X < 0)
        {
            spriteBody.FlipH = true;
        }

        if (dashTimer.TimeLeft == 0)
        {
            velocity = velocity / 2;
            Rotation = 0;
            animationPlayer.Play("RESET");
            spriteBody.FlipH = false;
            currentPlayerState = State.FLYING;
        }
    }

    public void _Process_Flying(double delta)
    {
        GetMovementVector();
        GetVelocity();
        SetMovementAnimations();
    }

    public void _Process_Stunned(double delta)
    {
        GetMovementVector();
        GetVelocity();

        velocity = velocity * 0.90f;

        if (stunnedTimer.TimeLeft == 0)
        {
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
        { velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, speedDelta); }
        else
        { velocity.X = Mathf.Lerp(velocity.X, 0, speedDelta); }

        if (direction.Y < 0)
        { velocity.Y = Mathf.Lerp(velocity.Y, direction.Y * speed, speedDelta); }

        if (direction.Y > 0)
        { velocity.Y = Mathf.Lerp(velocity.Y, (direction.Y * speed) * declinePush, speedDelta); }
        else
        { velocity.Y = Mathf.Lerp(velocity.Y, 0, speedDelta); }

        if (Mathf.Abs(direction.X) + Mathf.Abs(direction.Y) > 1)
        {
            velocity = velocity * 0.98f;  //slow down diagonal movement slightly
        }
    }

    private void SetMovementAnimations()
    {
        if (direction.Y < 0)  //going up
        {
            if (hasPackage)
            { animationPlayer.Play("flying-up-package"); }
            else
            { animationPlayer.Play("flying-up"); }
        }
        else if (direction.Y > 0 && direction.X > 0)  //going down and forward
        {
            if (hasPackage)
            { animationPlayer.Play("flying-down-forward-package"); }
            else
            { animationPlayer.Play("flying-down-forward"); }
        }
        else if (direction.Y > 0 && direction.X == 0)  //going only down
        {
            if (hasPackage)
            { animationPlayer.Play("flying-down-package"); }
            else
            { animationPlayer.Play("flying-down"); }
        }
        else if (direction.Y > 0 || direction.X < 0)  //going down or backward
        {
            if (hasPackage)
            { animationPlayer.Play("flying-down-backward-package"); }
            else
            { animationPlayer.Play("flying-down-backward"); }
        }
        else
        {
            if (hasPackage)
            { animationPlayer.Play("flying-idle-package"); }
            else
            { animationPlayer.Play("flying-idle"); }
        }
    }

    private void DropPackage()
    {
        hasPackage = false;
        CharacterBody2D newPackage = (CharacterBody2D)package.Instantiate();
        GetParent().AddChild(newPackage);
        newPackage.GlobalPosition = GlobalPosition;
        newPackage.GlobalPosition = new Vector2(GlobalPosition.X + 60, GlobalPosition.Y);
        recollectPackageTimer.Start();
    }

    private void OnPackageAreaEntered(object _)
    { nearPackage = true; }
    private void OnPackageAreaExited(object _)
    { nearPackage = false; }

    private void OnEnemyAreaEntered(object _)
    {
        if (hasPackage)
        {
            DropPackage();
        }
        stunnedTimer.Start();
        // CallDeferred("SetToStunned");
    }

    private Vector2 GetMousePosAgainstPlayerPosVector()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 localPos = mousePos - GlobalPosition;
        mouseVector = localPos.Normalized();
        return mouseVector;
    }

    private void SetToStunned()
    {
        currentPlayerState = State.STUNNED;
    }
}