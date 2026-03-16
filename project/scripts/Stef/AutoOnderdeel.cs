using Godot;
using System;

[GlobalClass]
public partial class AutoOnderdeel : Resource
{
    [Export] public string PartName;
    [Export] public PackedScene Onderdeel;
    [Export] public float Weight;
}
