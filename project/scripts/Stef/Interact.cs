using Godot;
using System;

public partial class Interact : Control
{
    private Label _tekstLabel;
    public int SelectedLanguage;

    public override void _Ready()
    {
        var data = Gamedata.LoadGame();
        SelectedLanguage = data.SelectedLanguage;
        _tekstLabel = GetNodeOrNull<Label>("InteractLabel");

        if (_tekstLabel == null)
        {
            GD.PrintErr("FOUT: Geen node gevonden met de naam 'InteractLabel' onder de Interact node!");
        }

        Hide();
    }

    public void ToonMelding(string onderdeel, string tekst, int prijs = 0)
    {
        if (_tekstLabel != null)
        {
            switch (SelectedLanguage)
            {
                case 0:
                    if (tekst == "buy")
                    {
                        _tekstLabel.Text = $"buy {onderdeel} for ${prijs}";
                        Show();
                    }
                    else if (tekst == "pick")
                    {
                        _tekstLabel.Text = $"pick up {onderdeel}";
                        Show();
                    }
                    else if (tekst == "place")
                    {
                        _tekstLabel.Text = $"place {onderdeel}";
                        Show();
                    }
                    break;
                case 2:
                    if (tekst == "buy")
                    {
                        _tekstLabel.Text = $"koop {onderdeel} voor ${prijs}";
                        Show();
                    }
                    else if (tekst == "pick")
                    {
                        _tekstLabel.Text = $"neem {onderdeel} vast";
                        Show();
                    }
                    else if (tekst == "place")
                    {
                        _tekstLabel.Text = $"plaats {onderdeel}";
                        Show();
                    }
                    break;
                case 3:
                    if (tekst == "buy")
                    {
                        _tekstLabel.Text = $"koop {onderdeel} voor ${prijs}";
                        Show();
                    }
                    else if (tekst == "pick")
                    {
                        _tekstLabel.Text = $"neem {onderdeel} vast";
                        Show();
                    }
                    else if (tekst == "place")
                    {
                        _tekstLabel.Text = $"plaats {onderdeel}";
                        Show();
                    }
                    break;
                case 4:
                    if (tekst == "buy")
                    {
                        _tekstLabel.Text = $"koop {onderdeel} voor ${prijs}";
                        Show();
                    }
                    else if (tekst == "pick")
                    {
                        _tekstLabel.Text = $"neem {onderdeel} vast";
                        Show();
                    }
                    else if (tekst == "place")
                    {
                        _tekstLabel.Text = $"plaats {onderdeel}";
                        Show();
                    }
                    break;
            }
        }
    }

    public void VerbergMelding()
    {
        Hide();
    }
}