using Godot;
using System;

public partial class ParallaxBackground : Godot.ParallaxBackground
{
    [Export] float xScrollSpeed;

    float xOffset;


    public override void _Ready()
    {
        xOffset = ScrollOffset.X;
    }

    public override void _Process(double delta)
    {
        ScrollOffset = new Vector2(xOffset, ScrollOffset.Y);
        xOffset -= xScrollSpeed;
    }
}
