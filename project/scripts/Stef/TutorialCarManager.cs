using Godot;
using System;

public partial class TutorialCarManager : Node3D
{
	[Export] public PackedScene AutoScene;
	[Export] public float WachttijdSeconden = 0.0f;
	[Export] public Vector3 SpawnPositie = new Vector3(-4, 0, 2);
	[Export] public Vector3 SpawnRotatie = new Vector3(0, -90, 0);
	[Export] public Vector3 SpawnSchaal = new Vector3(0.2f, 0.2f, 0.2f);
	private Node3D _huidigeAuto = null;
	[Export] public MoneyManager MoneySystem;
	[Export] public int BeloningPerAuto = 250;

	public override void _Ready()
	{
		SpawnNieuweAuto();
	}

	private void SpawnNieuweAuto()
	{
		if (AutoScene == null) return;

		_huidigeAuto = AutoScene.Instantiate<Node3D>();
		AddChild(_huidigeAuto);

		_huidigeAuto.GlobalPosition = SpawnPositie;
		_huidigeAuto.RotationDegrees = SpawnRotatie;
		_huidigeAuto.Scale = SpawnSchaal;

		Node mogelijkeScriptNode = _huidigeAuto;

		if (!(mogelijkeScriptNode is AutoWerking))
		{
			mogelijkeScriptNode = _huidigeAuto.FindChild("*", true, false);
		}
		if (mogelijkeScriptNode is AutoWerking script)
		{
			script.AutoVoltooid += OnAutoVoltooid;
			script.RandomizeAuto();
			GD.Print("Auto succesvol gespawnd en script gekoppeld.");
		}
		else
		{
			GD.PrintErr("FOUT: CarManager kon geen AutoWerking script vinden op de auto!");
		}
	}

	private async void OnAutoVoltooid()
	{
		Node scriptNode = _huidigeAuto;
		if (!(scriptNode is AutoWerking)) scriptNode = _huidigeAuto.FindChild("*", true, false);

		if (scriptNode is AutoWerking script)
		{
			script.AutoVoltooid -= OnAutoVoltooid;
		}
		GD.Print("Auto voltooid! Beloning uitbetalen...");
		if (MoneySystem != null) MoneySystem.AddMoney(BeloningPerAuto);
		GD.PrintErr("FOUT: AnimationPlayer niet gevonden op de auto!");
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		if (IsInstanceValid(_huidigeAuto))
		{
			GD.Print("Auto wordt nu verwijderd.");
			_huidigeAuto.QueueFree();
			_huidigeAuto = null;
		}
		if (WachttijdSeconden > 0)
		{
			GD.Print($"Wachten op volgende auto ({WachttijdSeconden}s)...");
			await ToSignal(GetTree().CreateTimer(WachttijdSeconden), "timeout");
		}

		SpawnNieuweAuto();
	}
}
