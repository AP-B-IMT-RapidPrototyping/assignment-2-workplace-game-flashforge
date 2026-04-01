using Godot;
using System;

public partial class NPCInteractie : Control
{
	[Export] public Button AcceptButton;
	[Export] public Button RejectButton;
	[Export] public Button WaitButton;
	[Export] public Label TekstLabel;

	[Export] public Node CarManagerNode;
    [Export] public PlayerController PlayerNode;
    private bool _isGeaccepteerd = false;

    public override void _Ready()
    {
        if (PlayerNode == null)
        {
            PlayerNode = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
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
        if (TekstLabel != null) TekstLabel.Text = "Succes met de reparatie!";
        AcceptButton?.Hide();
        RejectButton?.Hide();
        SluitMenu();
    }

    public void _on_reject_pressed()
    {
        if (CarManagerNode != null)
        {
            if (CarManagerNode is TutorialCarManager tcm) tcm.VerwijderAutoDirect();
            else if (CarManagerNode is Carmanager cm) cm.VerwijderAutoDirect();
        }
        SluitMenu();
    }

    public void _on_wait_pressed() => SluitMenu();

    private void SluitMenu()
    {
        this.Hide();
        if (PlayerNode != null)
        {
            PlayerNode.SetMenuState(false);
        }
        else
        {
            GD.PrintErr("NPCInteractie: PlayerNode is null!");
        }
    }

    public void ResetInteractie()
    {
        _isGeaccepteerd = false;
        AcceptButton?.Show();
        RejectButton?.Show();
        if (TekstLabel != null) TekstLabel.Text = "Wil je deze auto repareren?";
    }

    public void OverschrijfTekst(string nieuweTekst)
    {
        if (TekstLabel != null)
        {
            TekstLabel.Text = nieuweTekst;
        }
        else
        {
            GD.Print("tekstlabel niet gevonden");
        }
    }

    public void ToonKnoppen(bool accept, bool reject)
    {
        if (AcceptButton != null) AcceptButton.Visible = accept;
        if (RejectButton != null) RejectButton.Visible = reject;
    }
}