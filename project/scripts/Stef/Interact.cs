using Godot;
using System;

public partial class Interact : Label
{
	[Export] private Label _tekstLabel;

	public override void _Ready()
	{
		_tekstLabel = GetNode<Label>("InteractLabel"); // Zorg dat de naam klopt
		Hide(); // Begin onzichtbaar
	}

	public void ToonMelding(string tekst)
	{
		if (_tekstLabel != null)
		{
			_tekstLabel.Text = tekst;
		}
		Show(); // Toont zowel het label als het TextureRect kind
	}

	public void VerbergMelding()
	{
		Hide(); // Verbergt alles tegelijk
	}
}