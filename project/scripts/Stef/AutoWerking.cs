using Godot;
using System;

public partial class AutoWerking : Node
{
    [Export] public float MaxFuel = 100f;
    [Export] public float CurrentFuel = 0;

    public override void _Ready()
    {
    }

    public void InteractieMetOnderdeel(Node3D collider, AutoResource nieuwOnderdeel)
    {
        if (collider.GetParent() is Marker3D slot)
        {
            GD.Print($"Onderdeel vervangen in slot: {slot.Name}");
            InstalleerOnderdeel(slot, nieuwOnderdeel);
        }
    }

    public void InstalleerOnderdeel(Marker3D slot, AutoResource data)
    {
        foreach (Node child in slot.GetChildren())
        {
            if (child is not StaticBody3D) 
            {
                child.Free();
            }
        }

        if (data != null && data.OnderdeelModel != null)
        {
            var nieuwModel = data.OnderdeelModel.Instantiate<Node3D>();
            slot.AddChild(nieuwModel);
            nieuwModel.Position = Vector3.Zero;
            nieuwModel.Rotation = Vector3.Zero;
            nieuwModel.Scale = Vector3.One;
        }
    }
}
