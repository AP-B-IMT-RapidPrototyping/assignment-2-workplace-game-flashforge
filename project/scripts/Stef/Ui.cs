using Godot;
using System;

public partial class Ui : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var typewriter = GetNode<Tekstfunctie>("MijnRichTextLabel");
		typewriter.SpeelTekstAf("Hallo daar! Welkom in mijn game. Hopelijk vind je het leuk? Veel succes!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
