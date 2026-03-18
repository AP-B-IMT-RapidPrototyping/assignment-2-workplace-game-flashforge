using Godot;
using System;

public partial class Carmanager : Node3D
{
	[Export] public PackedScene AutoScene;
	[Export] public float WachttijdSeconden = 10.0f;
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
		var script = _huidigeAuto.FindChild("*", true, false) as AutoWerking;
		if (script != null)
		{
			script.AutoVoltooid -= OnAutoVoltooid;
		}
		GD.Print("Auto wordt over 2 seconden weggehaald...");
		if (MoneySystem != null)
		{
			MoneySystem.AddMoney(BeloningPerAuto);
		}
		await ToSignal(GetTree().CreateTimer(2.0f), "timeout");

		if (IsInstanceValid(_huidigeAuto))
		{
			_huidigeAuto.QueueFree();
			_huidigeAuto = null;
		}

		GD.Print($"Wachten op volgende auto ({WachttijdSeconden}s)...");
		await ToSignal(GetTree().CreateTimer(WachttijdSeconden), "timeout");

		SpawnNieuweAuto();
	}
}