using Godot;
using System;

public partial class Snelwegauto : PathFollow3D
{
    [Export] public float Speed = 5.0f;

    public override void _Process(double delta)
    {
        Progress += Speed * (float)delta;
    }
}
