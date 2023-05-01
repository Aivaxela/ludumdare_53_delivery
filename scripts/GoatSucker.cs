using Godot;
using System;

public partial class GoatSucker : CharacterBody2D
{
    [Export] int cruiseSpeedMin;
    [Export] int cruiseSpeedMax;
    [Export] float launchSpeed;
    [Export] int hp;

    [Export] Timer timeUntilLaunchTimer;
    [Export] Timer timeUntilDestroyTimer;
    [Export] Sprite2D sprite;
    [Export] Sprite2D wingSprite;
    [Export] AnimationPlayer animationPlayer;
    [Export] Area2D playerAttackArea;
    [Export] PackedScene hitMarker;
    [Export] AudioStreamPlayer deathSFX;
    [Export] AudioStreamPlayer dashSFX;
    [Export] GpuParticles2D deathParticles;

    LevelManager levelManager;
    Vector2 velocity;
    Vector2 direction;
    float cruiseSpeed;
    float currentXPos1;
    float currentXPos2;
    float currentYPos1;
    float currentYPos2;
    int bounceCount = 0;

    bool directionLocked = false;
    bool destroyTimerStarted = false;
    bool checkPos1 = true;
    bool launched = false;
    bool dashSFXplayed = false;


    public override void _Ready()
    {
        levelManager = GetNode<LevelManager>("/root/LevelManager");

        timeUntilLaunchTimer.WaitTime = GD.RandRange(1, 4);
        timeUntilLaunchTimer.Start();
        direction = new Vector2(-1, 0);
        cruiseSpeed = GD.RandRange(cruiseSpeedMin, cruiseSpeedMax);

        playerAttackArea.BodyEntered += OnPlayerAttackEntered;
    }

    public override void _Process(double delta)
    {
        if (timeUntilLaunchTimer.TimeLeft == 0)
        {
            DirectTowardsPlayer();
            velocity = direction * launchSpeed;
            CollisionMask = 16;
            launched = true;
            animationPlayer.Play("attacking");
            if (!destroyTimerStarted)
            {
                timeUntilDestroyTimer.Start();
                destroyTimerStarted = true;
            }
            if (!dashSFXplayed)
            {
                dashSFX.Play();
                dashSFXplayed = true;
            }
        }
        else
        {
            velocity = direction * cruiseSpeed;
        }

        CheckForWallCollision();

        if (velocity.X > 0)
        {
            sprite.FlipV = false;
            wingSprite.FlipV = false;
        }

        if (timeUntilDestroyTimer.TimeLeft == 0 && destroyTimerStarted)
        {
            QueueFree();
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
        }
    }

    private void CheckForWallCollision()
    {
        if (checkPos1)
        {
            currentXPos1 = GlobalPosition.X;
            currentYPos1 = GlobalPosition.Y;
            checkPos1 = false;
        }
        else
        {
            currentXPos2 = GlobalPosition.X;
            currentYPos2 = GlobalPosition.Y;
            checkPos1 = true;
        }
        if (launched)
        {
            if (bounceCount == 4)
            {
                CollisionMask = 0;
            }
            else if (Mathf.Abs(currentXPos1 - currentXPos2) < 0.1 || Mathf.Abs(currentYPos1 - currentYPos2) < 0.1)
            {
                directionLocked = false;
                DirectTowardsPlayer();
                velocity = direction * launchSpeed;
                bounceCount++;
            }
        }
    }

    private void OnPlayerAttackEntered(object objHitBy)
    {
        Node2D newHitMarker = (Node2D)hitMarker.Instantiate();
        GetParent().AddChild(newHitMarker);
        newHitMarker.GlobalPosition = GlobalPosition;
        CharacterBody2D objHitByBody2D = (CharacterBody2D)objHitBy;
        newHitMarker.Rotation = objHitByBody2D.Velocity.Angle();
        objHitByBody2D.QueueFree();

        hp -= 1;
        if (hp == 0)
        {
            RemoveChild(deathSFX);
            GetParent().AddChild(deathSFX);
            RemoveChild(deathParticles);
            GetParent().AddChild(deathParticles);
            deathSFX.PitchScale = (float)GD.RandRange(0.8, 1.2);
            deathSFX.Play();
            deathParticles.GlobalPosition = GlobalPosition;
            deathParticles.Emitting = true;
            levelManager.levelEnemiesLeft -= 1;
            QueueFree();
        }
    }
}
