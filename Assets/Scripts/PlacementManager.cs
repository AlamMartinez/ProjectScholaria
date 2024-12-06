using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the placement of Buildings and paths
/// </summary>
public class PlacementManager
{
    private GameManager gameManager;
    private BuildingManager buildingManager;
    private Grid grid;
    private int placementMode;
    private List<Cell> busStops;


    public PlacementManager(GameManager gameManager, BuildingManager buildingManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.buildingManager = buildingManager;
        this.grid = grid;
        placementMode = NONE;
        busStops = new List<Cell>();
    }
    /// <summary>
    /// Will attempt to place a Building or path (depending on placementMode) at the given
    /// position. If the position is clear, instruct the corresponding manager to actually
    /// place the structure.
    /// </summary>
    /// <param name="position">Grid position to place the structure at</param>
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
                    //Debug.Log("Cannot place path at (" + position.x + ", " + position.y + ")");
                    return;
                }
                gameManager.AddPath(position.x, position.y);
                break;
            case ROAD:
                if (grid.GetCell(position.x, position.y).GetType() != Cell.EMPTY)
                {
                    Debug.Log("Cannot place road at (" + position.x + ", " + position.y + ")");
                    return;
                }
                gameManager.AddRoad(position.x, position.y);
                break;
            case BUS_STOP:
                if (grid.GetCell(position.x, position.y).GetType() == Cell.ROAD)
                {
                    gameManager.DemolishRoad(position);
                    gameManager.AddBusStop(position.x, position.y);
                    busStops.Add(grid.GetCell(position.x, position.y));
                    return;
                }
                Debug.Log("Missing road for bus stop at (" + position.x + ", " + position.y + ")");
                break;
            case CROSS_WALK:
                if (grid.GetCell(position.x, position.y).GetType() == Cell.EMPTY)
                {
                    //gameManager.DemolishRoad(position);
                    gameManager.AddCrossWalk(position.x, position.y);
                    //busStops.Add(grid.GetCell(position.x, position.y));
                    return;
                }
                Debug.Log("Missing road for bus stop at (" + position.x + ", " + position.y + ")");
                break;
        }
    }
    /// <summary>
    /// Will demolish the structure that occupies the given Cell.
    /// </summary>
    /// <param name="position"></param>
    public void DemolishAt(Vector2Int position)
    {
        if(grid.GetCell(position.x, position.y).GetType() == Cell.EMPTY)
        {
            return;
        }
        Debug.Log("Demolishing " + position.x + ", " + position.y + ": " + grid.GetCell(position.x, position.y).GetType());
        switch (grid.GetCell(position.x, position.y).GetType())
        {
            case Cell.BUILDING:
                buildingManager.DemolishBuilding(position);
                break;
            case Cell.PATH:
                gameManager.DemolishPath(position);
                break;
            case Cell.ROAD:
                gameManager.DemolishRoad(position);
                break;
            case Cell.BUS_STOP:
                for (int i = 0; i < busStops.Count; i++)
                {
                    if (grid.GetCell(position.x, position.y).GetX() == busStops[i].GetX() &&
                        grid.GetCell(position.x, position.y).GetY() == busStops[i].GetY())
                    {
                        busStops.RemoveAt(i);
                        break;
                    }
                }
                gameManager.DemolishBusStop(position);
                break;
            case Cell.CROSS_WALK:
                gameManager.DemolishCrossWalk(position);
                break;
        }
    }
    public int GetPlacementMode() { return placementMode; }
    public int GetBusStopCount() { return busStops.Count; }

    public Cell GetRandomBusStop()
    {
        if (busStops.Count > 0)
        {
            return busStops[Random.Range(0, busStops.Count)];
        }
        return null;
    }

    public List<Cell> GetBusStops()
    {
        return busStops;
    }

    public void SetPlacementMode(int mode) { placementMode = mode; }
    //Cycle between placement variants, i.e. building blueprints
    public void Cycle(int amount)
    {
        switch (placementMode)
        {
            case BUILDING:
                buildingManager.IncrementBuildingSelection(amount);
                break;
        }
    }
    public const int NONE = 0;
    public const int BUILDING = 1;
    public const int PATHING = 2;
    public const int ROAD = 3;
    public const int BUS_STOP = 4;
    public const int CROSS_WALK = 5;
}
