using Godot;
using System;
using System.Threading.Tasks;

public partial class TutorialUI : Control
{
    [Export] public Label TutorialLabel;
    [Export] public Label GeldLabel;
    [Export] public TutorialCarManager CarManager;
    [Export] public NPCManager NPCManager;
    [Export] public NPCInteractie NPCMenu;
	[Export] public PlayerController PlayerNode;

    private int _tutorialStap = 0;
    private bool _wachtOpInput = false;

    public override void _Ready()
    {
        Visible = true;
        GeldLabel?.Hide();
        TutorialLabel.Text = "";
        
        tutorial_1();
    }

    public override void _Input(InputEvent @event)
    {
        if (_wachtOpInput && @event.IsActionPressed("ui_accept"))
        {
            _wachtOpInput = false;
        }
    }

    private async Task WachtOpEnter()
    {
        _wachtOpInput = true;
        while (_wachtOpInput)
        {
            await ToSignal(GetTree(), "process_frame");
        }
    }

    private async Task Wacht(float seconden)
    {
        await ToSignal(GetTree().CreateTimer(seconden), "timeout");
    }

    public async void tutorial_1()
    {
        TutorialLabel.Text = "Welkom bij de garage! In dit spel repareer je auto's voor klanten.\n(Druk op ENTER om verder te gaan)";
        await WachtOpEnter();
        
        TutorialLabel.Text = "Kijk naar de auto voor je om te zien welke onderdelen er missen.\n(Druk op ENTER om verder te gaan)";
        await WachtOpEnter();
        tutorial_2();
    }

    public async void tutorial_2()
{
    TutorialLabel.Text = "Pak een van de ontbrekende onderdelen op met 'E'. Deze staan naast de auto geplaatst";
    
    if (PlayerNode != null)
    {
        await ToSignal(PlayerNode, PlayerController.SignalName.ItemOpgepakt);
        await Wacht(0.5f);
        tutorial_3();
    }
    else
    {
        GD.PrintErr("FOUT: PlayerNode is niet gekoppeld in de Inspector van TutorialUI!");
        
        PlayerController backupPlayer = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
        if (backupPlayer != null)
        {
            await ToSignal(backupPlayer, PlayerController.SignalName.ItemOpgepakt);
            await Wacht(0.5f);
            tutorial_3();
        }
    }
}

    public async void tutorial_3()
{
    TutorialLabel.Text = "Plaats het onderdeel nu op de juiste plaats op de auto.";
    
    if (PlayerNode != null)
    {
        await ToSignal(PlayerNode, PlayerController.SignalName.ItemGeplaatst);
    }
    else
    {
        PlayerController backupPlayer = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
        if (backupPlayer != null) await ToSignal(backupPlayer, PlayerController.SignalName.ItemGeplaatst);
    }

    await Wacht(0.5f);
    TutorialLabel.Text = "Maak de auto nu volledig af door alle missende onderdelen te plaatsen.";
    
    if (CarManager != null)
    {
        GD.Print("Tutorial wacht op CarManager signaal...");
        await ToSignal(CarManager, TutorialCarManager.SignalName.AutoKlaar); 
        GD.Print("AutoKlaar ontvangen!");
    }
    else
    {
        GD.PrintErr("FOUT: CarManager is niet gekoppeld in de TutorialUI Inspector!");
    }
    
    tutorial_4();
}

    public async void tutorial_4()
    {
        GeldLabel.Text = "Geld: $0";
        GeldLabel.Show();
        
        TutorialLabel.Text = "Je krijgt geld voor elke reparatie. In het echte spel kosten onderdelen ook geld.\n(Wachten op volgende klant...)";
        
        await Wacht(6.0f);
        
        tutorial_5();
    }

   public async void tutorial_5()
{
    TutorialLabel.Text = "Daar is een klant! Praat met de NPC om het bod te bekijken.";
    
    NPCManager.IsTutorialMode = true;

    NPCManager.SetTutorialTekst("Ik geef je $5 voor deze hele auto. (Lachwekkend laag bod)");
    if (NPCMenu != null)
    {
        NPCMenu.ResetInteractie();
        NPCMenu.AcceptButton.Hide();
        NPCMenu.RejectButton.Show();
    }

    await ToSignal(CarManager, TutorialCarManager.SignalName.AutoVerwijderd);

    
    NPCManager.SetTutorialTekst("Deze reparatie is belangrijk, ik bied $500!");
    if (NPCMenu != null)
    {
        NPCMenu.ResetInteractie();
        NPCMenu.AcceptButton.Show();
        NPCMenu.RejectButton.Hide();
    }

    await ToSignal(CarManager, TutorialCarManager.SignalName.AutoKlaar);
    tutorial_6();
}

    public async void tutorial_6()
    {
        GeldLabel.Text = "Geld: $500";
        TutorialLabel.Text = "Tutorial voltooid! Je bent nu een echte monteur.\nDruk op ESC en ga naar het hoofdmenu om het echte spel te starten.";
        
        var data = Gamedata.LoadGame();
        Gamedata.SaveGame(
            data.Money,
            data.NPCSetting,
            data.SelectedLanguage, 
            1,
            data.TutorialActive
        );
    }
}