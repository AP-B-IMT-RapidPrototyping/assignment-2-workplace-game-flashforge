using Godot;
using System;

public partial class Pauzemenu : Node3D
{
	[Export] public CanvasLayer PauzePaneel;
    private bool _isGepauzeerd = false;
	[Export] public MoneyManager GeldBeheerder; 

    public void _on_hoofdmenu_pressed()
    {
        if (GeldBeheerder != null)
        {
            Gamedata.SaveGame(GeldBeheerder.CurrentMoney);
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
