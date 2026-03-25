using Godot;
using System;

public partial class MenuInteractable : Area3D
{
    [Export] public AudioStreamPlayer3D Audio;

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
    }

    private void OnInputEvent(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            GD.Print("Interactie in menu!");
            SpeelInteractieAf();
        }
    }

    private void SpeelInteractieAf()
    {
        if (Audio != null)
        {
			Audio.VolumeDb = -15.0f;
            Audio.Play();
        }
    }
}