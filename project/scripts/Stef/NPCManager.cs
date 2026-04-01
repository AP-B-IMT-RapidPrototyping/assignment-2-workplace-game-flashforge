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
    [Export] public Control PrijsLabel;
	[Export] public Label RepairText;
    [Export] public CollisionShape3D NPCInteractie;
	[Export] public NPCInteractie InteractieMenu;
    [Export] public bool IsTutorialMode = false;
private string _tutorialTekstOverride = "";

    private int _huidigeAutoPrijs = 0;
    private Random _random = new Random();
    private bool _isAutoBezig = false;
    private Node3D _huidigeZichtbareModel = null;
    private int _huidigeSessieId = 0;

    public override void _Ready()
    {
        VerbergAlleModellen(MaleCharacters);
        VerbergAlleModellen(FemaleCharacters);
        if (PrijsLabel != null) PrijsLabel.Hide();
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

    public void StelPrijsIn(int prijs)
    {
        _huidigeAutoPrijs = prijs;
    }

    public void ToonPrijsOpUI()
{
    if (PrijsLabel != null && _huidigeZichtbareModel != null && _huidigeZichtbareModel.Visible)
    {
        if (IsTutorialMode && !string.IsNullOrEmpty(_tutorialTekstOverride))
        {
            RepairText.Text = _tutorialTekstOverride;
        }
        else
        {
            RepairText.Text = $"I can give you ${_huidigeAutoPrijs} for the repair";
        }
        PrijsLabel.Show();
    }
}
public void SetTutorialTekst(string tekst)
{
    _tutorialTekstOverride = tekst;
}

    public void VerbergPrijsUI()
    {
        if (PrijsLabel != null) PrijsLabel.Hide();
    }

    private void VerbergAlleModellen(Godot.Collections.Array<Node3D> lijst)
    {
        if (lijst == null) return;
    foreach (Node3D model in lijst) model?.Hide();
    
    if (NPCInteractie != null) 
    {
        NPCInteractie.Disabled = true; 
    }
    }

    public async void NPCSpawn()
    {
		if (_huidigeZichtbareModel != null)
        {
            _huidigeZichtbareModel.Hide();
            _huidigeZichtbareModel = null;
        }
        if (NPCInteractie != null) NPCInteractie.Disabled = true;
        
        if (InteractieMenu != null) InteractieMenu.ResetInteractie();
        _isAutoBezig = false;
        _huidigeSessieId++;
        int mijnSessieId = _huidigeSessieId;
        _isAutoBezig = true;

        int geslacht = _random.Next(0, 2); 
        Godot.Collections.Array<AudioStream> gekozenLijst = null;
        Godot.Collections.Array<Node3D> gekozenLijstModels = (geslacht == 0) ? MaleCharacters : FemaleCharacters;

        int setKeuze = _random.Next(0, 3);
        if (geslacht == 0) {
            switch (setKeuze) {
                case 0: gekozenLijst = MaleVoicelines_English_1; break;
                case 1: gekozenLijst = MaleVoicelines_English_2; break;
                case 2: gekozenLijst = MaleVoicelines_English_3; break;
            }
        } else {
            switch (setKeuze) {
                case 0: gekozenLijst = FemaleVoicelines_English_1; break;
                case 1: gekozenLijst = FemaleVoicelines_English_2; break;
                case 2: gekozenLijst = FemaleVoicelines_English_3; break;
            }
        }

        if (gekozenLijstModels != null && gekozenLijstModels.Count > 0)
    {
        _huidigeZichtbareModel = gekozenLijstModels[_random.Next(0, gekozenLijstModels.Count)];
        var data = Gamedata.LoadGame();
        if (data.TutorialActive == 1){
        await ToSignal(GetTree().CreateTimer(12.0f), "timeout");
        }
        if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) return;
        
        _huidigeZichtbareModel.Show();

        if (NPCInteractie != null)
        {
            NPCInteractie.Disabled = false;
        }
    }

        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
        if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) return;

        SpeelVoiceline(gekozenLijst, true);

        while (_isAutoBezig && mijnSessieId == _huidigeSessieId)
        {
            await ToSignal(GetTree().CreateTimer(_random.Next(30, 61)), "timeout");
            if (!_isAutoBezig || mijnSessieId != _huidigeSessieId) break;
            SpeelVoiceline(gekozenLijst, false);
        }
    }

    public void NPCDespawn()
    {
        _isAutoBezig = false;
        _huidigeSessieId++;
        if (VoicePlayer != null) VoicePlayer.Stop();
        if (PrijsLabel != null) PrijsLabel.Hide();
        if (_huidigeZichtbareModel != null) _huidigeZichtbareModel.Hide();
        if (NPCInteractie != null) NPCInteractie.Hide();
        _huidigeZichtbareModel = null;
    }

    private void SpeelVoiceline(Godot.Collections.Array<AudioStream> lijst, bool isEerste)
    {
        if (lijst == null || lijst.Count == 0 || VoicePlayer == null) return;
        VoicePlayer.Stream = isEerste ? lijst[0] : lijst[_random.Next(1, lijst.Count)];
        VoicePlayer.Play();
    }
}