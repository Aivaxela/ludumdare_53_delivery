using Godot;
using System;

public partial class MusicPersist : Node
{
    AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();


    public override void _Ready()
    {
        CallDeferred(nameof(CreateAudioStreamPlayerAndPlay));
    }

    private void CreateAudioStreamPlayerAndPlay()
    {
        GetTree().Root.AddChild(audioStreamPlayer);
        audioStreamPlayer.Stream = GD.Load<AudioStream>("res://music/sanctuary showdown (guitar replaced).wav");
        audioStreamPlayer.VolumeDb = -2;
        audioStreamPlayer.Play();
    }

    public void ChangeMusicToBattle()
    {
        audioStreamPlayer.Stop();
        audioStreamPlayer.Stream = GD.Load<AudioStream>("res://music/hasty delivery(3).wav");
        audioStreamPlayer.Play();
    }
    public void ChangeMusicToEnd()
    {
        audioStreamPlayer.Stop();
        audioStreamPlayer.Stream = GD.Load<AudioStream>("res://music/end(1).wav");
        audioStreamPlayer.Play();
    }
}
