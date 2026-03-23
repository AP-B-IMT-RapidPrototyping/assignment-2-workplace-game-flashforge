using Godot;
using System;

public partial class NPCManager : Node3D
{
	[Export] public Godot.Collections.Array<Node3D> MaleCharacters;
	[Export] public Godot.Collections.Array<Node3D> FemaleCharacters;

	[Export] public Godot.Collections.Array<AudioStream> MaleVoicelines_English;
    [Export] public Godot.Collections.Array<AudioStream> FemaleVoicelines_English;

    [Export] public AudioStreamPlayer3D VoicePlayer;
    [Export] public Node3D PlayerToFollow;

    private Random _random = new Random();
	private bool _isAutoBezig = false;
	private Node3D _huidigeZichtbareModel = null;
	public override void _Ready()
    {
        VerbergAlleModellen(MaleCharacters);
        VerbergAlleModellen(FemaleCharacters);
    }
    public override void _Process(double delta)
    {
        if (PlayerToFollow != null && _huidigeZichtbareModel != null && IsInstanceValid(_huidigeZichtbareModel))
        {
            Vector3 targetPosition = PlayerToFollow.GlobalPosition;
            targetPosition.Y = _huidigeZichtbareModel.GlobalPosition.Y;

            _huidigeZichtbareModel.LookAt(targetPosition, Vector3.Up);
            _huidigeZichtbareModel.RotateY(Mathf.DegToRad(180));
        }
    }
	private void VerbergAlleModellen(Godot.Collections.Array<Node3D> lijst)
    {
        if (lijst == null) return;
        foreach (Node3D model in lijst)
        {
            model?.Hide();
        }
    }
	public async void NPCSpawn()
    {
        GD.Print("NPC spawning begint");
        int geslacht = _random.Next(0, 2); 
		_isAutoBezig = true;
		var gekozenLijst = (geslacht == 0) ? MaleVoicelines_English : FemaleVoicelines_English;
		var gekozenLijstModels = (geslacht == 0) ? MaleCharacters : FemaleCharacters;

		if (gekozenLijstModels != null && gekozenLijstModels.Count > 0)
        {
            int modelId = _random.Next(0, gekozenLijstModels.Count);
            _huidigeZichtbareModel = gekozenLijstModels[modelId];
            
            if (IsInstanceValid(_huidigeZichtbareModel))
            {
                await ToSignal(GetTree().CreateTimer(12.0f), "timeout");
                _huidigeZichtbareModel.Show();
                GD.Print($"Model {_huidigeZichtbareModel.Name} is nu zichtbaar.");
            }
        }
		await ToSignal(GetTree().CreateTimer(12.0f), "timeout");
        SpeeleersteVoiceline(gekozenLijst);

        while (_isAutoBezig)
        {
            float wachtTijd = (float)_random.NextDouble() * (15.0f - 8.0f) + 8.0f;
            await ToSignal(GetTree().CreateTimer(wachtTijd), "timeout");

            if (!_isAutoBezig) break;

            SpeelVoiceline(gekozenLijst);
        }
        GD.Print("NPC stopt met praten, auto is weg.");
    }
    public void NPCDespawn()
    {
        StopPraten();

        if (_huidigeZichtbareModel != null && IsInstanceValid(_huidigeZichtbareModel))
        {
            _huidigeZichtbareModel.Hide();
            GD.Print($"NPC Model {_huidigeZichtbareModel.Name} is nu onzichtbaar.");
            _huidigeZichtbareModel = null;
        }
    }

    public void StopPraten()
    {
        _isAutoBezig = false;
        if (VoicePlayer != null && VoicePlayer.IsPlaying())
        {
            VoicePlayer.Stop();
        }
    }
    private void SpeeleersteVoiceline(Godot.Collections.Array<AudioStream> lijst)
    {
        if (lijst == null || lijst.Count == 0 || VoicePlayer == null) return;
        VoicePlayer.Stream = lijst[0];
        VoicePlayer.Play();
    }
    private void SpeelVoiceline(Godot.Collections.Array<AudioStream> lijst)
    {
        if (lijst == null || lijst.Count <= 1 || VoicePlayer == null) return;
        GD.Print("eerste voiceline afgespeeld");
        int index = _random.Next(1, lijst.Count);
        VoicePlayer.Stream = lijst[index];
        VoicePlayer.Play();
    }
}
