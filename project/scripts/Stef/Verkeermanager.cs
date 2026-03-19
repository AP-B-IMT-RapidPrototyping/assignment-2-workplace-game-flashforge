using Godot;
using System;

public partial class Verkeermanager : Node
{
    [Export] public PackedScene AutoScene;
    [Export] public Node3D PathParent;
    [Export] public float SpawnInterval = 2.0f;

    public override void _Ready()
    {
		GD.Print("timer werkt");
        Timer spawnTimer = new Timer();
        spawnTimer.WaitTime = SpawnInterval;
        spawnTimer.Autostart = true;
        spawnTimer.Timeout += OnSpawnTimeout;
        AddChild(spawnTimer);
    }

    private void OnSpawnTimeout()
    {
		GD.Print("auto gespawned");
        if (PathParent == null || AutoScene == null) return;

        var paden = PathParent.GetChildren();
        if (paden.Count == 0) return;

        var gekozenPad = paden[GD.RandRange(0, paden.Count - 1)] as Path3D;

        if (gekozenPad != null)
        {
            var nieuweAuto = AutoScene.Instantiate<Node3D>(); 
            gekozenPad.AddChild(nieuweAuto);
            
            if (nieuweAuto is Snelwegauto autoScript)
            {
                autoScript.Progress = 0;
            }
        }
    }
}
