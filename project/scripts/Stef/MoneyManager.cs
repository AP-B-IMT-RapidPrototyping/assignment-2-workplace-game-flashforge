using Godot;
using System;

public partial class MoneyManager : Node
{
	[Export] public int StartingMoney { get; set; } = 1000;

	public int CurrentMoney { get; private set; }
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount);
	public override void _Ready()
	{
		int geladenGeld = Gamedata.LoadGame();
		CurrentMoney = (geladenGeld > 0) ? geladenGeld : StartingMoney;
		UpdateUI();
	}
	public bool CanAfford(int cost) => CurrentMoney >= cost;
	public bool TrySpend(int amount)
	{
		if (!CanAfford(amount))
		{
			GD.Print($"Not enough money! Need {amount}, have {CurrentMoney}");
			return false;
		}
		CurrentMoney -= amount;
		EmitSignal(SignalName.MoneyChanged, CurrentMoney);
		GD.Print($"Spent {amount}. Remaining: {CurrentMoney}");
		return true;
	}
	public void AddMoney(int amount)
	{
		CurrentMoney += amount;
		EmitSignal(SignalName.MoneyChanged, CurrentMoney);
		GD.Print($"Earned {amount}. Total: {CurrentMoney}");
	}
	public void UpdateUI()
{
    EmitSignal(SignalName.MoneyChanged, CurrentMoney);
}
}
