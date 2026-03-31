using Godot;
using System;
using System.Threading.Tasks;

public partial class TutorialUI : Control
{
    [Export] public Label TutorialLabel;
    [Export] public Label GeldLabel;
    [Export] public Carmanager CarManager;
    [Export] public NPCManager NPCManager;
    [Export] public NPCInteractie NPCMenu;

    private int _tutorialStap = 0;
    private bool _wachtOpInput = false;

    public override void _Ready()
    {
        Visible = true;
        GeldLabel?.Hide();
        TutorialLabel.Text = "";
        
        // Start de eerste stap
        tutorial_1();
    }

    public override void _Input(InputEvent @event)
    {
        if (_wachtOpInput && @event.IsActionPressed("ui_accept")) // Enter of Space
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
        TutorialLabel.Text = "Loop naar de rekken en pak een onderdeel op met 'E'.";
        
        // We wachten tot de speler iets vastheeft
        PlayerController player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
        while (player != null && !player.IsHoldingSomething())
        {
            await ToSignal(GetTree(), "process_frame");
        }
        
        tutorial_3();
    }

    public async void tutorial_3()
    {
        TutorialLabel.Text = "Goed zo! Loop nu naar de auto en plaats het onderdeel op de juiste plek.";
        
        // Wacht tot de auto een onderdeel ontvangt (we checken of de speler het loslaat)
        PlayerController player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
        while (player != null && player.IsHoldingSomething())
        {
            await ToSignal(GetTree(), "process_frame");
        }

        TutorialLabel.Text = "Maak de auto nu volledig af door alle missende onderdelen te plaatsen.";
        
        // Wacht op signaal van CarManager dat de auto klaar is
        await ToSignal(CarManager, "AutoKlaar"); // Zorg dat Carmanager dit signaal emit
        
        tutorial_4();
    }

    public async void tutorial_4()
    {
        GeldLabel.Text = "Geld: $0";
        GeldLabel.Show();
        
        TutorialLabel.Text = "Je krijgt geld voor elke reparatie. In het echte spel kosten onderdelen ook geld.\n(Wachten op volgende klant...)";
        
        await ToSignal(GetTree().CreateTimer(3f), "timeout");
        CarManager.SpawnNieuweAuto(); // Forceer een nieuwe spawn
        
        tutorial_5();
    }

    public async void tutorial_5()
    {
        TutorialLabel.Text = "Daar is een klant! Praat met de NPC om het bod te bekijken.";
        
        // Stap 5a: Reject testen
        if (NPCMenu != null)
        {
            NPCMenu.AcceptButton.Hide();
            NPCMenu.RejectButton.Show();
            NPCMenu.TekstLabel.Text = "Ik geef je $5 voor deze hele auto. (Lachwekkend laag bod)";
        }

        // Wacht tot de auto weggestuurd wordt (Reject pressed)
        // Je moet in Carmanager een signaal 'AutoVerwijderd' maken
        await ToSignal(CarManager, "AutoVerwijderd");

        TutorialLabel.Text = "Goed gedaan. Sommige biedingen zijn te laag. Wacht op de volgende klant.";
        await ToSignal(GetTree().CreateTimer(4f), "timeout");
        CarManager.SpawnNieuweAuto();

        // Stap 5b: Accept testen
        if (NPCMenu != null)
        {
            NPCMenu.AcceptButton.Show();
            NPCMenu.RejectButton.Hide();
            NPCMenu.TekstLabel.Text = "Deze reparatie is belangrijk, ik bied $500!";
        }

        // Wacht tot de auto weer voltooid is
        await ToSignal(CarManager, "AutoKlaar");
        tutorial_6();
    }

    public async void tutorial_6()
    {
        GeldLabel.Text = "Geld: $500";
        TutorialLabel.Text = "Tutorial voltooid! Je bent nu een echte monteur.\nDruk op ESC en ga naar het hoofdmenu om het echte spel te starten.";
        
        // Opslaan dat tutorial klaar is
        var data = Gamedata.LoadGame();
        data.TutorialCompleted = 1;
        Gamedata.SaveGame(data);
    }
}
