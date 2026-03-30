using Godot;
using System;

public partial class CashRegister : Node3D
{
    [Export] public Control ShopUI;

    private bool playerInRange = false;

    public override void _Process(double delta)
    {
        if (playerInRange && Input.IsActionJustPressed("interact"))
        {
            ShopUI.Visible = true;
        }
    }

    private void _on_area_3d_body_entered(Node body)
    {
        if (body.Name == "Player")
            playerInRange = true;
    }

    private void _on_area_3d_body_exited(Node body)
    {
        if (body.Name == "Player")
            playerInRange = false;
    }
}