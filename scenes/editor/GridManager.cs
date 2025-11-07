using System.Collections.Generic;
using System.Linq;
using Game.Component;
using Godot;

namespace Game.Manager;

public partial class GridManager : Node
{
	[Export]
	private TileMapLayer highlightTileMapLayer;
	[Export]
	private TileMapLayer baseTerrainTileMapLayer;

	private HashSet<Vector2I> occupiedCells = new();

	public void MarkTileAsOccupied(Vector2I tilePosition)
	{
		occupiedCells.Add(tilePosition);
	}

	public bool IsTilePositionValid(Vector2I tilePosition)
	{
		Vector2I tilePositionInt = new((int)tilePosition.X, (int)tilePosition.Y);
		TileData customData = baseTerrainTileMapLayer.GetCellTileData(tilePositionInt);

		if (customData == null) return false;

		bool isBuildable = (bool)customData.GetCustomData("buildable");
		return !occupiedCells.Contains(tilePosition) && isBuildable;
	}

	public void HighlightBuildableTiles()
	{
		ClearHighlightedTiles();
		IEnumerable<BuildingComponent> buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();

		foreach (BuildingComponent buildingComponent in buildingComponents)
		{
			HighlightValidTilesInRadius(buildingComponent.GetGridCellPosition(), buildingComponent.BuildableRadius);
		}
	}

	public void ClearHighlightedTiles()
	{
		highlightTileMapLayer.Clear();
	}

	public Vector2I GetMouseGridCellPostition()
	{
		Vector2 mousePosition = highlightTileMapLayer.GetGlobalMousePosition();
		Vector2 gridPosition = (mousePosition / 64).Floor();

		return new((int)gridPosition.X, (int)gridPosition.Y);
	}

	private void HighlightValidTilesInRadius(Vector2I rootCell, int radius)
	{
		for (int x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (int y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				Vector2I tilePosition = new(x, y);
				if (!IsTilePositionValid(tilePosition)) continue;
				highlightTileMapLayer.SetCell(tilePosition, 1, Vector2I.Zero);
			}
		}
	}
}