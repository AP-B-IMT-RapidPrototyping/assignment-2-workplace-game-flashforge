using Godot;
using System;

public partial class LanguageSettings : Control
{
	[Export] public Godot.Collections.Array<String> Languages;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
