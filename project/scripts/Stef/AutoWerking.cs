using Godot;
using System;

public partial class AutoWerking : Node
{
	[Export] public float MaxFuel = 100f;
    public float CurrentFuel;
    [Export] public Marker3D MotorSlot;

    public override void _Ready()
    {
        CurrentFuel = MaxFuel;
    }

    public void EquipPart(Marker3D slot, AutoOnderdeel part)
    {
        foreach (var child in slot.GetChildren())
        {
            child.QueueFree();
        }
        
        if (part?.Onderdeel != null)
        {
            var newPart = part.Onderdeel.Instantiate<Node3D>();
            slot.AddChild(newPart);
        }
    }
}
