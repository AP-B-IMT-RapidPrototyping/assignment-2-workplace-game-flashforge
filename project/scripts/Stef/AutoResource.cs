using Godot;

public enum OnderdeelCategorie { Wiel, Uitlaat, Motor, Stuur, Deur }

[GlobalClass]
public partial class AutoResource : Resource
{

	[Export] public string PartName;
	[Export] public PackedScene OnderdeelModel;
	[Export] public OnderdeelCategorie Type;
	[Export] public float Weight;
}