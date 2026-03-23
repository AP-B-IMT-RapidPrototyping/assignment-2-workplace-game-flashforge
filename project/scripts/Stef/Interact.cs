using Godot;
using System;

public partial class Interact : Control
{
    // Verwijder de [Export] hier, we vullen dit alleen via code in
    private Label _tekstLabel;

    public override void _Ready()
    {
        // Zoek de child node genaamd "InteractLabel"
        _tekstLabel = GetNodeOrNull<Label>("InteractLabel");

        if (_tekstLabel == null)
        {
            GD.PrintErr("FOUT: Geen node gevonden met de naam 'InteractLabel' onder de Interact node!");
        }

        Hide();
    }

    public void ToonMelding(string tekst)
    {
        if (_tekstLabel != null)
        {
            _tekstLabel.Text = tekst;
            Show(); // Alleen showen als de label ook echt bestaat
        }
    }

    public void VerbergMelding()
    {
        Hide();
    }
}