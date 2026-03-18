using Godot;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

public partial class AutoWerking : Node
{
    [Export] public float MaxFuel = 100f;
    [Export] public float CurrentFuel = 0;
    [Export] public Godot.Collections.Array<Marker3D> OnderdeelSlots;
    [Export] public Godot.Collections.Array<AutoResource> BeschikbareOnderdelen;
    [Export, Range(0, 1)] public float SpawnKans = 0.5f;
    public override void _Ready()
    {
        CallDeferred(nameof(RandomizeAuto));
    }
    public void RandomizeAuto()
    {
        Random random = new Random();
        GD.Print("Auto wordt gerandomized...");

        foreach (Marker3D slot in OnderdeelSlots)
        {
            var slotFysica = slot.GetNodeOrNull<AutoOnderdeel>("StaticBody3D");
            if (slotFysica == null) continue;

            if ((float)random.NextDouble() < SpawnKans)
            {
                var passendeOpties = new List<AutoResource>();
                foreach (var res in BeschikbareOnderdelen)
                {
                    if (res.Type == slotFysica.PastHierIn)
                    {
                        passendeOpties.Add(res);
                    }
                }
                if (passendeOpties.Count > 0)
                {
                    int index = random.Next(passendeOpties.Count);
                    InstalleerOnderdeel(slot, passendeOpties[index]);

                    slotFysica.OnderdeelData = passendeOpties[index];
                }
            }
            else
            {
                InstalleerOnderdeel(slot, null);
                slotFysica.OnderdeelData = null;
            }
        }
    }

    public void InteractieMetOnderdeel(Node3D collider, AutoResource nieuwOnderdeel)
    {
        if (collider is AutoOnderdeel slotFysica && collider.GetParent() is Marker3D slot)
        {
            if (nieuwOnderdeel != null && nieuwOnderdeel.Type != slotFysica.PastHierIn)
            {
                GD.Print($"FOUT: Een {nieuwOnderdeel.Type} past niet in een {slotFysica.PastHierIn} slot!");
                return;
            }

            GD.Print($"Onderdeel geplaatst in slot: {slot.Name}");
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
