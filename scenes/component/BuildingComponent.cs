using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export]
	public int BuildableRadius { get; private set; }

	public override void _Ready()
	{
		AddToGroup(nameof(BuildingComponent));
	}

	public Vector2I GetGridCellPosition()
	{
		Vector2 gridPosition = (GlobalPosition / 64).Floor();

		return new((int)gridPosition.X, (int)gridPosition.Y);
	}
}
