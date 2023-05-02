using Godot;
using System;
using System.Collections.Generic;

public partial class LevelManager : Node
{
    public int levelEnemiesLeft;
    int levelNumber = -1;

    List<string> levels = new List<string>();

    MusicPersist musicPersist;

    bool battleMusicStarted;
    bool endMusicStarted;


    public override void _Ready()
    {
        musicPersist = GetNode<MusicPersist>("/root/MusicPersist");

        levelEnemiesLeft = 30;

        levels.Add("res://scenes/ocean-level.tscn");
        levels.Add("res://scenes/main.tscn");
        levels.Add("res://scenes/end.tscn");
    }

    public override void _Process(double delta)
    {
        if (levelEnemiesLeft <= 0)
        {
            levelNumber++;
            levelEnemiesLeft = 30;
            GetTree().ChangeSceneToFile(levels[levelNumber]);
        }

        if (levelNumber == -1)
        {
            levelEnemiesLeft = 999;
        }

        if (levelNumber == 0 && !battleMusicStarted)
        {
            musicPersist.ChangeMusicToBattle();
            battleMusicStarted = true;
        }
        if (levelNumber == 2 && !endMusicStarted)
        {
            musicPersist.ChangeMusicToEnd();
            endMusicStarted = true;
        }
    }

    public void RetryLevel()
    {
        GetTree().ChangeSceneToFile(levels[levelNumber]);
    }
}

