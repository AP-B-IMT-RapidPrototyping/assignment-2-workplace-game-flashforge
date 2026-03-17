using Godot;

[GlobalClass]
public partial class AutoResource : Resource
{
	[Export] public string PartName;
	[Export] public PackedScene OnderdeelModel;
	[Export] public float Weight;
}
