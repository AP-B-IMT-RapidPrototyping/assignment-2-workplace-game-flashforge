using Godot;
using System;

public partial class TutorialCarManager : Node3D
{
    [Signal] public delegate void AutoKlaarEventHandler();
    [Signal] public delegate void AutoVerwijderdEventHandler();
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

    public void SpawnNieuweAuto()
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
            
            if (NPCManager != null)
            {
                NPCManager.StelPrijsIn(script.AutoBeloning);
                NPCManager.NPCSpawn();
            }
        }
    }

    private async void OnAutoVoltooid()
    {
        EmitSignal(SignalName.AutoKlaar);
        if (NPCManager != null) NPCManager.NPCDespawn();
        if (MoneySystem != null) MoneySystem.AddMoney(BeloningPerAuto);

        if (IsInstanceValid(_huidigeAuto)) _huidigeAuto.QueueFree();
        if (WachttijdSeconden > 0) await ToSignal(GetTree().CreateTimer(WachttijdSeconden), "timeout");
        SpawnNieuweAuto();
    }
    public void VerwijderAutoDirect()
{
    EmitSignal(SignalName.AutoVerwijderd);
    if (_huidigeAuto == null) return;

    if (NPCManager != null) NPCManager.NPCDespawn();
    
        _huidigeAuto.QueueFree();
        _huidigeAuto = null;
}
}