using Godot;
using System;
public partial class Opstartscherm : Control
{
    [Export] public PackedScene StartScene;
    [Export] public PackedScene Tutorial;
    [Export] public AnimationPlayer MenuAnimator;
    [Export] public MenuButton SelectedLanguage;
    private int _currentLanguageId = 0;

    [Export] public Button start;
    [Export] public Button settings;
    [Export] public Button quit;
    [Export] public Button tutorial;
    [Export] public Label Language;
    [Export] public Button credits;
    [Export] public CheckButton NPCKnop;
    public bool NPCSwitch = false;
    private int NPCSetting;
    public int TutorialCompleted;

    public override void _Ready()
    {
        NPCKnop.ButtonPressed = false;
        PopupMenu popup = SelectedLanguage.GetPopup();
        popup.IdPressed += OnMenuItemPressed;
        Gamedata.SaveData data = Gamedata.LoadGame();
        _currentLanguageId = data.SelectedLanguage;
        OnMenuItemPressed(_currentLanguageId);
        GD.Print($"Taal: {SelectedLanguage}");
    }
    private void OnMenuItemPressed(long id)
    {
        _currentLanguageId = (int)id;
        if (NPCSwitch == true)
        {
            NPCSetting = 1;
        }
        else
        {
            NPCSetting = 0;
        }
        var data = Gamedata.LoadGame();
        Gamedata.SaveGame(data.Money, _currentLanguageId, NPCSetting, TutorialCompleted);

        switch (id)
        {
            case 0:
                start.Text = "start";
                settings.Text = "settings";
                quit.Text = "quit";
                tutorial.Text = "tutorial";
                Language.Text = "language";
                SelectedLanguage.Text = "English";
                break;
            case 1:
                start.Text = "starten";
                settings.Text = "instellingen";
                quit.Text = "afluiten";
                tutorial.Text = "voorbeeld";
                Language.Text = "taal";
                SelectedLanguage.Text = "Nederlands";
                break;
            case 2:
                start.Text = "commencer";
                settings.Text = "paramètres";
                quit.Text = "quitter";
                tutorial.Text = "tutoriel";
                Language.Text = "langue";
                SelectedLanguage.Text = "Francais";
                break;
            case 3:
                start.Text = "start";
                settings.Text = "einstellungen";
                quit.Text = "aufhoren";
                tutorial.Text = "tutorial";
                Language.Text = "sprache";
                SelectedLanguage.Text = "Deutch";
                break;
        }
    }
    public void _on_start_pressed()
    {
        if (TutorialCompleted == 1)
        {
            GetTree().ChangeSceneToPacked(StartScene);
        }
        else if (TutorialCompleted == 0)
        {
            GetTree().ChangeSceneToPacked(Tutorial);
        }
    }
    public void _on_instellingen_pressed()
    {
        if (MenuAnimator != null)
        {
            MenuAnimator.Play("NaarInstellingen");
        }
    }
    public void _on_terug_pressed()
    {
        if (MenuAnimator != null)
        {
            MenuAnimator.Play("TerugNaarHoofdmenu");
        }
    }
    public void _on_check_button_toggled()
    {
        GD.Print("NPCs aan");
    }
    public void _on_check_button_button_up()
    {
        GD.Print("NPCs uit");
    }
    public void _on_afsluiten_pressed()
    {
        var data = Gamedata.LoadGame();
        Gamedata.SaveGame(data.Money, _currentLanguageId, NPCSetting, TutorialCompleted);
        GetTree().Quit();
    }
    public void _on_tutorial_pressed()
    {
        GetTree().ChangeSceneToPacked(Tutorial);
    }
    public void _on_check_button_pressed()
    {
        if (NPCSwitch == true)
        {
            NPCSwitch = false;
            GD.Print("NPC's uit");
        }
        else if (NPCSwitch == false)
        {
            NPCSwitch = true;
            GD.Print("NPC's aan");
        }
    }
    public void _on_credits_pressed()
    {
        GD.Print("credits geactiveerd");
    }
}
