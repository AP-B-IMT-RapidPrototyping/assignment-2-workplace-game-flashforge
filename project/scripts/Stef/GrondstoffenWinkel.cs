using Godot;
using System;

public partial class GrondstoffenWinkel : StaticBody3D
{
	[Export] public AutoResource OnderdeelData;
	[Export] public int Prijs = 50;
	[Export] public MoneyManager MoneySystem;
	public void KoopOnderdeel(PlayerController speler)
	{
		if (speler.IsHoldingSomething())
		{
			GD.Print("Handen vol!");
			return;
		}

		if (MoneySystem != null && MoneySystem.TrySpend(Prijs))
		{
			GD.Print("Betaald! Onderdeel spawnen...");
			speler.SpawnObjectInHand(OnderdeelData);
		}
		else
		{
			GD.Print("Niet genoeg geld of MoneySystem ontbreekt!");
		}
	}
}
