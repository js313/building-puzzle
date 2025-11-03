using System.Collections.Generic;
using Godot;

namespace Game;

public partial class Main : Node
{
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTileMapLayer;
	private Vector2? hoveredGridCell;

	private HashSet<Vector2> occupiedCells = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		highlightTileMapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent @evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !occupiedCells.Contains(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}

	private Vector2 GetMouseGridCellPostition()
	{
		Vector2 mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
		Vector2 gridPosition = (mousePosition / 64).Floor();

		return gridPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 gridPosition = GetMouseGridCellPostition();
		cursor.GlobalPosition = gridPosition * 64;

		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			UpdateHighlightTileMapLayer();
		}
	}

	private void UpdateHighlightTileMapLayer()
	{
		highlightTileMapLayer.Clear();

		if (!hoveredGridCell.HasValue) return;

		for (int x = (int)hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (int y = (int)hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTileMapLayer.SetCell(new Vector2I(x, y), 1, Vector2I.Zero);
			}
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;
		Node2D building = buildingScene.Instantiate<Node2D>();

		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		occupiedCells.Add(hoveredGridCell.Value);

		hoveredGridCell = null;
		UpdateHighlightTileMapLayer();
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
