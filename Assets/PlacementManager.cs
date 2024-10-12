using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager
{
    private GameManager gameManager;
    private BuildingManager buildingManager;
    private Grid grid;
    private int placementMode;

    public PlacementManager(GameManager gameManager, BuildingManager buildingManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.buildingManager = buildingManager;
        this.grid = grid;
        placementMode = NONE;
    }
    public void PlaceAt(Vector2Int position)
    {
        switch(placementMode)
        {
            case BUILDING:
                //Get current building template & check collisions
                BuildingTemplate template = buildingManager.GetCurrentTemplate();
                for(int x = 0; x < template.GetWidth(); x++)
                {
                    for(int y = 0; y < template.GetHeight(); y++)
                    {
                        if (template.GetCell (x,y) > 0 && grid.GetCell(position.x + x, position.y + y).GetType() != Cell.EMPTY)
                        {
                            Debug.Log("Cannot place building at (" + position.x + ", " + position.y + ")");
                            return;
                        }
                    }
                }
                //If building footprint is clear, create new building
                buildingManager.ConstructBuilding(position, template);
                break;
            case PATHING:
                if(grid.GetCell(position.x,position.y).GetType() != Cell.EMPTY)
                {
                    Debug.Log("Cannot place path at (" + position.x + ", " + position.y + ")");
                    return;
                }
                gameManager.AddPath(position.x, position.y);
                break;
        }
    }
    public void DemolishAt(Vector2Int position)
    {
        switch (grid.GetCell(position.x, position.y).GetType())
        {
            case Cell.BUILDING:
                buildingManager.DemolishBuilding(position);
                break;
            case Cell.PATH:
                gameManager.DemolishPath(position);
                break;
        }
    }
    public int GetPlacementMode() { return placementMode; }
    public void SetPlacementMode(int mode) { placementMode = mode; }
    //Cycle between placement variants, i.e. building blueprints
    public void Cycle(int amount)
    {
        switch(placementMode)
        {
            case BUILDING:
                buildingManager.IncrementBuildingSelection(amount);
                break;
        }
    }
    public const int NONE = 0;
    public const int BUILDING = 1;
    public const int PATHING = 2;
}
