using Godot;
using System;
using System.ComponentModel;

public partial class Carmanager : Node3D
{
	[Export] public PackedScene AutoScene;
	[Export] public float WachttijdSeconden = 0.0f;
	[Export] public Vector3 SpawnPositie = new Vector3(-4, 0, 1.75f);
	[Export] public Vector3 SpawnRotatie = new Vector3(0, -90, 0);
	[Export] public Vector3 SpawnSchaal = new Vector3(0.2f, 0.2f, 0.2f);
	private Node3D _huidigeAuto = null;
	[Export] public MoneyManager MoneySystem;
	[Export] public NPCManager NPCManager;
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
			AnimationPlayer anim = _huidigeAuto.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
			anim.Play("spawnanimation");
			if (NPCManager != null)
            {
                NPCManager.NPCSpawn();
            }
		}
	}

	private async void OnAutoVoltooid()
	{
		Node scriptNode = _huidigeAuto;
		if (!(scriptNode is AutoWerking)) scriptNode = _huidigeAuto.FindChild("*", true, false);

		if (scriptNode is AutoWerking script)
		{
			script.AutoVoltooid -= OnAutoVoltooid;
			int finaleBeloning = script.AutoBeloning; 
        
        if (MoneySystem != null) MoneySystem.AddMoney(finaleBeloning);
        GD.Print($"Auto voltooid! Beloning van ${finaleBeloning} uitbetaald.");
		}
		if (NPCManager != null)
    	{
        	NPCManager.NPCDespawn();
    	}
		AnimationPlayer anim = _huidigeAuto.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
		if (anim != null)
		{
			await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
			anim.Play("despawnanimation");
			await ToSignal(anim, "animation_finished");
		}
		else
		{
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		}
		if (IsInstanceValid(_huidigeAuto))
		{
			_huidigeAuto.QueueFree();
			_huidigeAuto = null;
		}
		if (WachttijdSeconden > 0)
		{
			await ToSignal(GetTree().CreateTimer(WachttijdSeconden), "timeout");
		}

		SpawnNieuweAuto();
	}
}