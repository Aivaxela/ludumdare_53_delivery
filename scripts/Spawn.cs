using Godot;
using System;

public partial class Spawn : Marker2D
{
    [Export] int spawnRateMin;
    [Export] int spawnRateMax;

    [Export] PackedScene enemyScene;
    [Export] Timer timeUntilSpawnTimer;


    public override void _Ready()
    {
        timeUntilSpawnTimer.WaitTime = GD.RandRange(spawnRateMin, spawnRateMax);
        timeUntilSpawnTimer.Start();
    }

    public override void _Process(double delta)
    {
        if (timeUntilSpawnTimer.TimeLeft == 0)
        {
            CallDeferred("SpawnEnemy");
            timeUntilSpawnTimer.WaitTime = GD.RandRange(spawnRateMin, spawnRateMax);
            timeUntilSpawnTimer.Start();
        }
    }

    private void SpawnEnemy()
    {
        CharacterBody2D newEnemy = (CharacterBody2D)enemyScene.Instantiate();
        GetParent().AddChild(newEnemy);
        newEnemy.GlobalPosition = GlobalPosition;
    }
}
