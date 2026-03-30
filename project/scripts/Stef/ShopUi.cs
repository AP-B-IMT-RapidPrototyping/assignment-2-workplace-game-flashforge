using Godot;
using System;

public partial class ShopUI : Control
{
    [Export] public GrondstoffenWinkel WheelsShop;
    [Export] public GrondstoffenWinkel UitlaatShop;
    [Export] public GrondstoffenWinkel MotorShop;

    private PlayerController speler;

    public override void _Ready()
    {
        speler = GetNode<PlayerController>("/root/Main/Player");
        Visible = false;
    }

    public void BuyWheels()
    {
        WheelsShop.KoopOnderdeel(speler);
    }

    public void BuyUitlaat()
    {
        UitlaatShop.KoopOnderdeel(speler);
    }

    public void BuyMotor()
    {
        MotorShop.KoopOnderdeel(speler);
    }
}