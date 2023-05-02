using Godot;
using System;

public partial class Whale : CharacterBody2D
{
    [Export] float launchSpeed;
    [Export] int hp;
    [Export] float speedDelta;

    [Export] Area2D playerAttackArea;
    [Export] PackedScene hitMarker;
    [Export] AudioStreamPlayer splashFX;
    [Export] AudioStreamPlayer deathSFX;
    [Export] GpuParticles2D deathParticles;
    [Export] Timer timeUntilDestroyTimer;

    LevelManager levelManager;
    Vector2 velocity;
    Vector2 direction;

    bool splashSFXplayed = false;


    public override void _Ready()
    {
        levelManager = GetNode<LevelManager>("/root/LevelManager");

        playerAttackArea.BodyEntered += OnPlayerAttackEntered;

        direction = new Vector2(0, -1);
    }

    public override void _Process(double delta)
    {
        velocity.Y = Mathf.Lerp(velocity.Y, direction.Y * launchSpeed, speedDelta);

        Velocity = velocity;
        MoveAndSlide();

        if (timeUntilDestroyTimer.TimeLeft == 0)
        {
            QueueFree();
        }
    }

    private void OnPlayerAttackEntered(object objHitByObj)
    {
        Node2D newHitMarker = (Node2D)hitMarker.Instantiate();
        GetParent().AddChild(newHitMarker);
        newHitMarker.GlobalPosition = GlobalPosition;
        Attack objHitBy = (Attack)objHitByObj;
        newHitMarker.Rotation = objHitBy.Velocity.Angle();

        if (!objHitBy.isBolt)
        {
            objHitBy.QueueFree();
        }

        if (objHitBy.isBolt)
        {
            hp -= 5;
        }
        else
        {
            hp -= 1;
        }

        if (hp <= 0)
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
