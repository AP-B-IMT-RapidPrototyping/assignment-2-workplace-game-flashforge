using Godot;
using System;

public partial class Pauzemenu : Node3D
{
	[Export] public CanvasLayer PauzePaneel;
    private bool _isGepauzeerd = false;
	[Export] public MoneyManager GeldBeheerder;
    public int SelectedLanguage;
    [Export] public Button GameContinue;
    [Export] public Button MainMenu;
    private int NPCSetting;

    public override void _Ready()
    {
        var data = Gamedata.LoadGame();
        SelectedLanguage = data.SelectedLanguage;
        GD.Print($"Taal: {SelectedLanguage}");
        switch (SelectedLanguage)
        {
            case 0:
                GameContinue.Text = "continue";
                MainMenu.Text = "quit to main menu";
                break;
            case 1:
                GameContinue.Text = "verder gaan";
                MainMenu.Text = "afsluiten naar hoofdmenu";
                break;
            case 2:
                GameContinue.Text = "continuer";
                MainMenu.Text = "retour au manu principal";
                break;
            case 3:
                GameContinue.Text = "weiterspielen";
                MainMenu.Text = "zuruck zum hauptmenu";
            break;
        }
    }
    public void _on_hoofdmenu_pressed()
    {
        if (GeldBeheerder != null)
        {
             var data = Gamedata.LoadGame();
            Gamedata.SaveGame(GeldBeheerder.CurrentMoney, SelectedLanguage, NPCSetting, data.TutorialCompleted, 0);
            GD.Print("Spel opgeslagen voor afsluiten.");
        }

        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scenes/Stef/opstartscherm.tscn");
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            WisselPauze();
        }
    }

    private void WisselPauze()
    {
        _isGepauzeerd = !_isGepauzeerd;
        GetTree().Paused = _isGepauzeerd;
        if (_isGepauzeerd)
        {
            PauzePaneel.Show();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            PauzePaneel.Hide();
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
    public void _on_hervatten_pressed()
    {
        WisselPauze();
    }
}
