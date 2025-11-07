using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private Vector2I? hoveredGridCell;

	private GridManager gridManager;

	public override void _Ready()
	{
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		gridManager = GetNode<GridManager>("GridManager");
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent @evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionValid(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		Vector2I gridPosition = gridManager.GetMouseGridCellPostition();
		cursor.GlobalPosition = gridPosition * 64;

		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			gridManager.HighlightBuildableTiles();
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;
		Node2D building = buildingScene.Instantiate<Node2D>();

		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		gridManager.MarkTileAsOccupied(hoveredGridCell.Value);

		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
