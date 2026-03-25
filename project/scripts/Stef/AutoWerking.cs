using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public partial class AutoWerking : Node
{
    [Signal] public delegate void AutoVoltooidEventHandler();

    [Export] public Godot.Collections.Array<Marker3D> OnderdeelSlots;
    [Export] public Godot.Collections.Array<AutoResource> BeschikbareOnderdelen;
    [Export, Range(0, 1)] public float SpawnKans = 0.5f;
    private int AantalOnderdelen = 0;

    public override void _Ready()
    {
        CallDeferred(nameof(InitialiseerAuto));
    }
    private void InitialiseerAuto()
    {
        if (OnderdeelSlots == null || OnderdeelSlots.Count == 0)
        {
            foreach (Node child in GetChildren())
            {
                if (child is Marker3D marker) OnderdeelSlots.Add(marker);
            }
        }

        RandomizeAuto();
    }

    public void RandomizeAuto()
    {
        Random random = new Random();

        int verplichtLeegSlotIndex = random.Next(OnderdeelSlots.Count);

        for (int i = 0; i < OnderdeelSlots.Count; i++)
        {
            Marker3D slot = OnderdeelSlots[i];
            var slotFysica = slot.GetNodeOrNull<AutoOnderdeel>("StaticBody3D");
            if (slotFysica == null) continue;

            if (i == verplichtLeegSlotIndex)
            {
                InstalleerOnderdeel(slot, null);
                slotFysica.OnderdeelData = null;
                continue;
            }

            if ((float)random.NextDouble() < SpawnKans)
            {
                var passendeOpties = BeschikbareOnderdelen.Where(res => res.Type == slotFysica.PastHierIn).ToList();
                if (passendeOpties.Count > 0)
                {
                    var gekozen = passendeOpties[random.Next(passendeOpties.Count)];
                    InstalleerOnderdeel(slot, gekozen);
                    slotFysica.OnderdeelData = gekozen;
                    AantalOnderdelen++;
                    GD.Print(AantalOnderdelen);
                }
            }
            else
            {
                InstalleerOnderdeel(slot, null);
                slotFysica.OnderdeelData = null;
            }
        }
    }

    public void CheckOfCompleet()
    {
        bool isCompleet = true;
        foreach (Marker3D slot in OnderdeelSlots)
        {
            var slotFysica = slot.GetNodeOrNull<AutoOnderdeel>("StaticBody3D");
            if (slotFysica == null || slotFysica.OnderdeelData == null)
            {
                isCompleet = false;
                break;
            }
        }

        if (isCompleet)
        {
            GD.Print("Auto is compleet! Signaal verzenden...");
            AantalOnderdelen = 0;
            EmitSignal(SignalName.AutoVoltooid);
        }
    }
    public void InteractieMetOnderdeel(Node3D collider, AutoResource nieuwOnderdeel)
    {
        if (collider is AutoOnderdeel slotFysica && collider.GetParent() is Marker3D slot)
        {
            slotFysica.OnderdeelData = nieuwOnderdeel;
            InstalleerOnderdeel(slot, nieuwOnderdeel);

            CheckOfCompleet();
        }
    }

    public void InstalleerOnderdeel(Marker3D slot, AutoResource data)
    {
        foreach (Node child in slot.GetChildren())
        {
            if (child is not StaticBody3D)
            {
                child.QueueFree();
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



    public void SetTutorialVisuals(bool active)
    {
        var alleMeshes = FindChildren("*", "MeshInstance3D", true);

        foreach (Node node in alleMeshes)
        {
            if (node.Name.ToString().Contains("Placeholder"))
            {
                if (node is MeshInstance3D p)
                {
                    p.Visible = active;

                    if (active) GD.Print("Placeholder gevonden en geactiveerd: " + p.GetPath());
                }
            }
        }
    }
}