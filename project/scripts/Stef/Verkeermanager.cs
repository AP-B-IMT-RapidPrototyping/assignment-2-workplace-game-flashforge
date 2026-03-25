using Godot;
using System;

public partial class Verkeermanager : Node
{
    [Export] public Godot.Collections.Array<PackedScene> AutoVariaties;

    [Export] public Node3D PathParent;
    private Timer spawnTimer;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    [Export] public float MinWachttijd = 1.0f;
    [Export] public float MaxWachttijd = 4.0f;

    public override void _Ready()
    {
        Timer spawnTimer = new Timer();
        spawnTimer.Autostart = true;
        spawnTimer.Timeout += OnSpawnTimeout;
        AddChild(spawnTimer);
    }

    private void OnSpawnTimeout()
    {
        if (PathParent == null || AutoVariaties == null || AutoVariaties.Count == 0) return;

        var alleNodes = PathParent.FindChildren("*", "Path3D", true);
        if (alleNodes.Count == 0) return;

        var gekozenPad = alleNodes[GD.RandRange(0, alleNodes.Count - 1)] as Path3D;

        if (gekozenPad != null)
        {
            int randomIndex = GD.RandRange(0, AutoVariaties.Count - 1);
            PackedScene gekozenScene = AutoVariaties[randomIndex];

            var nieuweAutoNode = gekozenScene.Instantiate<Node3D>();

            if (nieuweAutoNode is Snelwegauto autoScript)
            {
                gekozenPad.AddChild(autoScript);
                autoScript.Progress = 0;
            }
        }
        float nieuweTijd = _rng.RandfRange(MinWachttijd, MaxWachttijd);
        spawnTimer.WaitTime = nieuweTijd;
    }
}