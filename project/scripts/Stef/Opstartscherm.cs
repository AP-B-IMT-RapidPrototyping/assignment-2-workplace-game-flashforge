using Godot;
using System;
public partial class Opstartscherm : Control
{
	[Export] public PackedScene StartScene;
    [Export] public AnimationPlayer MenuAnimator;
    public void _on_start_pressed()
    {
        if (StartScene != null)
        {
            GetTree().ChangeSceneToPacked(StartScene);
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
	public void _on_check_button_button_toggled()
	{
		GD.Print ("NPCs aan");
	}
	public void _on_check_button_button_up()
	{
		GD.Print ("NPCs uit");
	}
    public void _on_afsluiten_pressed()
    {
        GetTree().Quit();
    }
}
