using Godot;
using System;

public partial class AutoWerking : Node
{
    [Export] public float MaxFuel = 100f;
    public float CurrentFuel;

    public override void _Ready()
    {
        CurrentFuel = MaxFuel;
    }

    public void InteractieMetOnderdeel(Node3D collider, AutoOnderdeel nieuwOnderdeel)
    {
        if (collider.GetParent() is Marker3D slot)
        {
            GD.Print($"Onderdeel vervangen in slot: {slot.Name}");
            EquipPart(slot, nieuwOnderdeel);
        }
    }

    public void EquipPart(Marker3D slot, AutoOnderdeel part)
    {
        foreach (var child in slot.GetChildren())
        {
            if (child is not StaticBody3D) 
            {
                child.QueueFree();
            }
        }
        
        if (part?.Onderdeel != null)
        {
            var newPart = part.Onderdeel.Instantiate<Node3D>();
            slot.AddChild(newPart);
        }
    }
}
