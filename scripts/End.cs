using Godot;
using System;

public partial class End : Node
{
    [Export] Player player;
    [Export] AnimatedSprite2D packageSprite;


    public override void _Ready()
    {
        player.currentPlayerState = Player.State.END;
        player.animationPlayer.Play("flying-idle-package");
    }

    public void PackageDelivered()
    {
        player.RemoveChild(packageSprite);
        GetParent().AddChild(packageSprite);
        packageSprite.GlobalPosition = player.GlobalPosition;
    }

    public void LoadEndScene()
    {
        GetTree().ChangeSceneToFile("res://scenes/end-bg.tscn");
    }
}