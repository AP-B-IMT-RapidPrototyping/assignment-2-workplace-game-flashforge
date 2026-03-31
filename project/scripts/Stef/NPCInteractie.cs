using Godot;
using System;

public partial class NPCInteractie : Control
{
	[Export] public Button AcceptButton;
	[Export] public Button RejectButton;
	[Export] public Button WaitButton;
	[Export] public Label TekstLabel;

	private Carmanager _carManager;
    private PlayerController _player;
    private bool _isGeaccepteerd = false;

    public override void _Ready()
    {
        _carManager = GetTree().Root.FindChild("Carmanager", true, false) as Carmanager;
    
    // Zoek de speler op een veiligere manier
    	_player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
    
    // Als de groep niet werkt, probeer dan de oude manier met een extra check
    	if (_player == null)
    	{
        	_player = GetTree().Root.FindChild("Player", true, false) as PlayerController;
    	}
    }

    public void _on_accept_pressed()
    {
        if (_isGeaccepteerd) 
        {
            SluitMenu();
            return;
        }

        _isGeaccepteerd = true;

        if (TekstLabel != null) 
            TekstLabel.Text = "Succes met de reparatie!";

        AcceptButton.Hide();
        RejectButton.Hide();

        SluitMenu(); // <--- Zorgt dat speler weer kan lopen
    }

    public void _on_reject_pressed()
    {
        if (_carManager != null)
        {
            _carManager.VerwijderAutoDirect();
        }

        SluitMenu(); // <--- Zorgt dat speler weer kan lopen
    }

    public void _on_wait_pressed()
    {
        SluitMenu(); // <--- Zorgt dat speler weer kan lopen
    }

    private void SluitMenu()
    {
        GD.Print("SluitMenu aangeroepen"); // Debug check in je console
    this.Hide();
    
    if (_player != null)
    {
        GD.Print("Player gevonden, SetMenuState(false) wordt uitgevoerd");
        _player.SetMenuState(false);
    }
    else
    {
        GD.PrintErr("FOUT: NPCInteractie kan de PlayerController niet vinden!");
    }
	}

    public void ResetInteractie()
    {
        _isGeaccepteerd = false;
        if (AcceptButton != null) AcceptButton.Show();
        if (RejectButton != null) RejectButton.Show();
        if (TekstLabel != null)
            TekstLabel.Text = "Wil je deze auto repareren?";
    }
}