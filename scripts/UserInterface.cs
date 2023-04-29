using Godot;
using System;

public partial class UserInterface : CanvasLayer
{
    [Export] Label carryingPackageLabel;

    Player player;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/main/player");
    }

    public override void _Process(double delta)
    {
        if (player.hasPackage)
        {
            carryingPackageLabel.Text = "You have the package!";
        }
        else
        {
            carryingPackageLabel.Text = "You don't have the package!";
        }
    }
}
