using Godot;
using System;

public partial class InputManager : Node
{
    public bool usingController = false;


    public override void _Process(double delta)
    {
        Godot.Collections.Array<int> connectedJoypads = Input.GetConnectedJoypads();

        if (connectedJoypads.Count >= 1 && Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.LeftY)) >= 0.5 || Mathf.Abs(Input.GetJoyAxis(0, JoyAxis.LeftX)) >= 0.5)
        {
            usingController = true;
        }
        else if (connectedJoypads.Count == 0 || Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.D))
        {
            usingController = false;
        }
    }

    public bool UsingController()
    {
        if (usingController)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
