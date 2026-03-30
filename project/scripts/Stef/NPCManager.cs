using Godot;
using System;

public partial class NPCManager : Node3D
{
	[Export] public Godot.Collections.Array<Node3D> MaleCharacters;
	[Export] public Godot.Collections.Array<Node3D> FemaleCharacters;

	[Export] public Godot.Collections.Array<AudioStream> MaleVoicelines_English_1;
    [Export] public Godot.Collections.Array<AudioStream> MaleVoicelines_English_2;
    [Export] public Godot.Collections.Array<AudioStream> MaleVoicelines_English_3;
    
    [Export] public Godot.Collections.Array<AudioStream> FemaleVoicelines_English_1;
    [Export] public Godot.Collections.Array<AudioStream> FemaleVoicelines_English_2;
    [Export] public Godot.Collections.Array<AudioStream> FemaleVoicelines_English_3;

    [Export] public AudioStreamPlayer3D VoicePlayer;
    [Export] public Node3D PlayerToFollow;

    private Random _random = new Random();
	private bool _isAutoBezig = false;
	private Node3D _huidigeZichtbareModel = null;
    private int _huidigeSessieId = 0;
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
        var data = Gamedata.LoadGame();
        if (data.NPCSetting == 0) 
        {
        GD.Print("NPC Spawn geannuleerd: NPCSetting staat op UIT.");
        return; // Stop de functie hier
        }
        _isAutoBezig = false;
		_huidigeSessieId++;
		int mijnSessieId = _huidigeSessieId;

		_isAutoBezig = true;
		int geslacht = _random.Next(0, 2); 
		Godot.Collections.Array<AudioStream> gekozenLijst = null;
		Godot.Collections.Array<Node3D> gekozenLijstModels = null;

    if (geslacht == 0)
    {
        gekozenLijstModels = MaleCharacters;
        
        int setKeuze = _random.Next(0, 2);
        switch (setKeuze)
        {
            case 0: gekozenLijst = MaleVoicelines_English_1; break;
            case 1: gekozenLijst = MaleVoicelines_English_2; break;
            case 2: gekozenLijst = MaleVoicelines_English_3; break;
        }
    }
    else if (geslacht == 1)
    {
        gekozenLijstModels = FemaleCharacters;
        int setKeuze = _random.Next(0, 1);
        switch (setKeuze)
        {
            case 0: gekozenLijst = FemaleVoicelines_English_1; break;
            case 1: gekozenLijst = FemaleVoicelines_English_2; break;
            case 2: gekozenLijst = FemaleVoicelines_English_3; break;
        }
    }
		if (gekozenLijstModels != null && gekozenLijstModels.Count > 0)
		{
			int modelId = _random.Next(0, gekozenLijstModels.Count);
			_huidigeZichtbareModel = gekozenLijstModels[modelId];
			
			if (IsInstanceValid(_huidigeZichtbareModel))
			{
				await ToSignal(GetTree().CreateTimer(12.0f), "timeout");
				if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) return;
				_huidigeZichtbareModel.Show();
			}
		}

		await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
		if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) return;

		SpeeleersteVoiceline(gekozenLijst);

		while (_isAutoBezig && mijnSessieId == _huidigeSessieId)
		{
			float wachtTijd = (float)_random.Next(30, 61);
			await ToSignal(GetTree().CreateTimer(wachtTijd), "timeout");

			if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) break;

			SpeelVoiceline(gekozenLijst);
		}
        GD.Print($"Sessie {mijnSessieId} is beëindigd.");
	}

	public void NPCDespawn()
	{
		StopPraten();

		if (_huidigeZichtbareModel != null && IsInstanceValid(_huidigeZichtbareModel))
		{
			_huidigeZichtbareModel.Hide();
			_huidigeZichtbareModel = null;
		}
	}

	public void StopPraten()
	{
		_isAutoBezig = false;
        _huidigeSessieId++;
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
		int index = _random.Next(1, lijst.Count);
		VoicePlayer.Stream = lijst[index];
		VoicePlayer.Play();
	}
}