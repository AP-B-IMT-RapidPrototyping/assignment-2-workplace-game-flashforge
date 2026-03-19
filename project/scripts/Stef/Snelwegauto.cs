using Godot;
using System;

public partial class Snelwegauto : PathFollow3D
{
    [Export] public float Speed = 5.0f;

    public override void _Process(double delta)
    {
        Progress += Speed * (float)delta;
        
        // Als je niet wilt dat ze eeuwig rondjes rijden, 
        // kun je ze verwijderen als ProgressRatio == 1.0
    }
}
