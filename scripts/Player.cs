using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public enum State { FLYING, DASHING, STUNNED, END }
    public State currentPlayerState;

    [Export] float speed;
    [Export] float speedDelta;
    [Export] float dashSpeed;
    [Export] float declinePush;

    [Export] public AnimationPlayer animationPlayer;
    [Export] Area2D packageArea;
    [Export] Area2D enemyArea;
    [Export] Timer recollectPackageTimer;
    [Export] Timer dashTimer;
    [Export] public Timer dashCooldownTimer;
    [Export] Timer attackCooldownTimer;
    [Export] public Timer attackL1CooldownTimer;
    [Export] Timer stunMaxRemovalTimer;
    [Export] public Timer stunnedTimer;
    [Export] Timer stunCooldownTimer;
    [Export] PackedScene attack;
    [Export] PackedScene attackL1;
    [Export] PackedScene package;
    [Export] Sprite2D spriteBody;
    [Export] GpuParticles2D stunParticles;
    [Export] AudioStreamPlayer storkBlastSFX;
    [Export] AudioStreamPlayer storkBoltSFX;
    [Export] AudioStreamPlayer stunnedSFX;

    [Export] public int dashCooldown;

    public Vector2 velocity = Vector2.Zero;
    public Vector2 direction = Vector2.Zero;
    public bool hasPackage = true;

    InputManager inputManager;
    Vector2 dashDirection;
    Vector2 mouseVector;
    public float stunDuration = 1f;
    public float boltCd = 5f;
    bool nearPackage = false;
    bool stunned = false;
    bool dashing = false;
    bool end = false;


    public override void _Ready()
    {
        inputManager = GetNode<InputManager>("/root/InputManager");

        currentPlayerState = State.FLYING;
        stunParticles.Emitting = false;
        stunMaxRemovalTimer.Start();

        packageArea.BodyEntered += OnPackageAreaEntered;
        packageArea.BodyExited += OnPackageAreaExited;
        enemyArea.AreaEntered += OnEnemyAreaEntered;
    }

    public override void _Process(double delta)
    {
        DepleteStunDuration();

        if (!end)
        {
            direction.X = Input.GetActionStrength("move-right") - Input.GetActionStrength("move-left");
            dashCooldown = (int)dashCooldownTimer.TimeLeft;
            boltCd = (int)attackL1CooldownTimer.TimeLeft;
        }

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
            case State.END:
                _Process_End(delta);
                break;
        }

        Velocity = velocity;
        MoveAndSlide();

        if (Input.IsActionPressed("attack") && attackCooldownTimer.TimeLeft == 0 && !end && !stunned && !dashing)
        {
            storkBlastSFX.PitchScale = (float)GD.RandRange(0.8f, 1.2f);
            storkBlastSFX.Play();
            DropPackage();
            CharacterBody2D newAttack = (CharacterBody2D)attack.Instantiate();
            GetParent().AddChild(newAttack);
            newAttack.GlobalPosition = GlobalPosition;
            attackCooldownTimer.Start();
        }

        if (Input.IsActionPressed("stork-bolt") && attackL1CooldownTimer.TimeLeft == 0 && !end && !stunned && !dashing)
        {
            storkBoltSFX.PitchScale = (float)GD.RandRange(0.8f, 1.2f);
            storkBoltSFX.Play();
            DropPackage();
            CharacterBody2D newAttack = (CharacterBody2D)attackL1.Instantiate();
            GetParent().AddChild(newAttack);
            newAttack.GlobalPosition = GlobalPosition;
            attackL1CooldownTimer.Start();
        }

        if (Input.IsActionJustPressed("toggle-package") && !end)
        {
            if (hasPackage == false && nearPackage == true && recollectPackageTimer.TimeLeft == 0 && !stunned)
            {
                CollectPackage();
            }

            else if (hasPackage == true)
            {
                DropPackage();
            }
        }

        if (Input.IsActionJustPressed("dash") && dashCooldownTimer.TimeLeft == 0 && !hasPackage && !dashing && !end && !stunned)
        {
            if (inputManager.UsingController())
            {
                float horizontalInput = Input.GetJoyAxis(0, JoyAxis.LeftX);
                float verticalInput = Input.GetJoyAxis(0, JoyAxis.LeftY);

                dashDirection = new Vector2(horizontalInput, verticalInput).Normalized();
                dashing = true;
            }
            else if (!inputManager.UsingController())
            {
                dashDirection = GetMousePosAgainstPlayerPosVector();
                dashing = true;
            }

            if (dashDirection != Vector2.Zero)
            {
                dashTimer.Start();
                dashCooldownTimer.Start();
                currentPlayerState = State.DASHING;
            }
        }
    }

    public void _Process_Dashing(double delta)
    {
        velocity = dashDirection * dashSpeed;
        if (velocity.X < 0)
        {
            spriteBody.FlipH = true;
        }

        if (nearPackage)
        {
            CollectPackage();
        }

        if (dashTimer.TimeLeft == 0)
        {
            velocity = velocity / 2;
            Rotation = 0;
            dashing = false;
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
        animationPlayer.Play("stunned");

        velocity = velocity * 0.90f;

        if (stunnedTimer.TimeLeft == 0)
        {
            stunParticles.Emitting = false;
            stunned = false;
            stunCooldownTimer.Start();
            animationPlayer.Play("RESET");
            currentPlayerState = State.FLYING;
        }
    }

    public void _Process_End(double delta)
    {
        end = true;
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

    private void OnPackageAreaEntered(object _)
    {
        nearPackage = true;
    }
    private void OnPackageAreaExited(object _)
    {
        nearPackage = false;
    }

    private void OnEnemyAreaEntered(object _)
    {
        if (stunCooldownTimer.TimeLeft == 0 && !dashing && !stunned)
        {
            stunnedSFX.Play();
            if (hasPackage)
            {
                CallDeferred(nameof(DropPackageSetToStunned));
            }
            else
            {
                CallDeferred(nameof(SetToStunned));
            }
        }
    }

    private void DropPackageSetToStunned()
    {
        stunnedTimer.WaitTime = stunDuration;
        stunnedTimer.Start();
        stunDuration += 0.5f;
        stunParticles.Emitting = true;
        stunned = true;
        DropPackage();
        currentPlayerState = State.STUNNED;
    }

    private void SetToStunned()
    {
        stunnedTimer.WaitTime = stunDuration;
        stunnedTimer.Start();
        stunDuration += 0.5f;
        stunned = true;
        stunParticles.Emitting = true;
        currentPlayerState = State.STUNNED;
    }

    private void DropPackage()
    {
        if (GetTree().GetNodesInGroup("package").Count == 0)
        {
            hasPackage = false;
            CharacterBody2D newPackage = (CharacterBody2D)package.Instantiate();
            GetParent().AddChild(newPackage);
            newPackage.GlobalPosition = GlobalPosition;
            newPackage.GlobalPosition = new Vector2(GlobalPosition.X + 60, GlobalPosition.Y);
            recollectPackageTimer.Start();
        }
    }

    private void CollectPackage()
    {
        if (recollectPackageTimer.TimeLeft == 0)
        {
            hasPackage = true;
            Package currentPackage = GetTree().GetFirstNodeInGroup("package") as Package;
            currentPackage.QueueFree();
        }
    }

    private Vector2 GetMousePosAgainstPlayerPosVector()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 localPos = mousePos - GlobalPosition;
        mouseVector = localPos.Normalized();
        return mouseVector;
    }

    private void DepleteStunDuration()
    {
        if (stunMaxRemovalTimer.TimeLeft == 0 && stunDuration > 1f)
        {
            stunDuration -= 0.1f;
            stunMaxRemovalTimer.Start();
        }
    }
}