using Godot;
using System;

public partial class MoneyDisplay : Label
{
	[Export] public MoneyManager MoneyManagerNode;

	public override void _Ready()
	{

		if (MoneyManagerNode == null)
		{
			MoneyManagerNode = GetTree().Root.FindChild("MoneyManager", true, false) as MoneyManager;
		}

		if (MoneyManagerNode != null)
		{
			MoneyManagerNode.MoneyChanged += OnMoneyChanged;

			UpdateLabel(MoneyManagerNode.CurrentMoney);
			GD.Print("MoneyDisplay: Succesvol verbonden met MoneyManager.");
		}
		else
		{
			GD.PrintErr("MoneyDisplay: KAN MONEYMANAGER NIET VINDEN!");
		}
	}

	private void OnMoneyChanged(int newAmount)
	{
		UpdateLabel(newAmount);
	}

	private void UpdateLabel(int amount)
	{
		Text = $"{amount}$";
		GD.Print($"Label geüpdatet naar: {Text}");
	}
}