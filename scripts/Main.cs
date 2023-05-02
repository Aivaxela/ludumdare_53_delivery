using Godot;
using System;

public partial class Main : Node
{
    LevelManager levelManager;


    public override void _Ready()
    {
        levelManager = GetNode<LevelManager>("/root/LevelManager");
    }

    public void EndIntroLevel()
    {
        levelManager.levelEnemiesLeft = 0;
    }
}
