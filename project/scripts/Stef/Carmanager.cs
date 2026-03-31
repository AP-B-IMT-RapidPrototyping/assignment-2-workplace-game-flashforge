using Godot;
using System;

public partial class Carmanager : Node3D
{
    [Export] public PackedScene AutoScene;
    [Export] public float WachttijdSeconden = 5.0f;
    [Export] public Vector3 SpawnPositie = new Vector3(-4, 0, 1.75f);
    [Export] public Vector3 SpawnRotatie = new Vector3(0, -90, 0);
    [Export] public Vector3 SpawnSchaal = new Vector3(0.2f, 0.2f, 0.2f);
    [Export] public MoneyManager MoneySystem;
    [Export] public int BeloningPerAuto = 250;
    [Export] public NPCManager NPCManager;
    
    private Node3D _huidigeAuto = null;

    public override void _Ready() => SpawnNieuweAuto();

    private void SpawnNieuweAuto()
    {
        if (AutoScene == null) return;
        _huidigeAuto = AutoScene.Instantiate<Node3D>();
        AddChild(_huidigeAuto);
        _huidigeAuto.GlobalPosition = SpawnPositie;
        _huidigeAuto.RotationDegrees = SpawnRotatie;
        _huidigeAuto.Scale = SpawnSchaal;

        Node scriptNode = _huidigeAuto is AutoWerking ? _huidigeAuto : _huidigeAuto.FindChild("*", true, false);
        
        if (scriptNode is AutoWerking script)
        {
            script.AutoVoltooid += OnAutoVoltooid;
            script.RandomizeAuto();
            
            AnimationPlayer anim = _huidigeAuto.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
            anim?.Play("spawnanimation");

            if (NPCManager != null)
            {
                NPCManager.StelPrijsIn(script.AutoBeloning);
                NPCManager.NPCSpawn();
            }
        }
    }

    private async void OnAutoVoltooid()
    {
        if (NPCManager != null) NPCManager.NPCDespawn();
        if (MoneySystem != null) MoneySystem.AddMoney(BeloningPerAuto);

        AnimationPlayer anim = _huidigeAuto?.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
        if (anim != null)
        {
            await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
            anim.Play("despawnanimation");
            await ToSignal(anim, "animation_finished");
        }

        if (IsInstanceValid(_huidigeAuto)) _huidigeAuto.QueueFree();
        if (WachttijdSeconden > 0) await ToSignal(GetTree().CreateTimer(WachttijdSeconden), "timeout");
        SpawnNieuweAuto();
    }
    public void VerwijderAutoDirect()
{
    if (_huidigeAuto == null) return;

    if (NPCManager != null) NPCManager.NPCDespawn();
    
    AnimationPlayer anim = _huidigeAuto.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
    if (anim != null)
    {
        anim.Play("despawnanimation");
    }
    else
    {
        _huidigeAuto.QueueFree();
        _huidigeAuto = null;
    }
}
}