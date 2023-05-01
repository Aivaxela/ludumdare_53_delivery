using Godot;
using System;

public partial class LevelManager : Node
{
    public int levelEnemiesLeft;
    int levelNumber = 1;

    bool ended = false;


    public override void _Ready()
    {
        levelEnemiesLeft = 20;
    }

    public override void _Process(double delta)
    {
        if (levelEnemiesLeft <= 0 && !ended)
        {
            GetTree().ChangeSceneToFile("res://scenes/end.tscn");
            ended = true;

            GD.Print("end level loaded");
        }
    }
}
