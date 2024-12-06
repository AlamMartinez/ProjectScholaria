using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
/// <summary>
/// The BuildingManager is responsible for the placement, removal, and operation of Buildings.
/// </summary>
public class BuildingManager
{
    private GameManager gameManager;
    private Grid grid;
    private List<Building> buildings;
    private List<BuildingTemplate> templates;
    private int currentTemplate;
    private int buildingIndex;

    private List<BuildingTemplate> buildingInventory;
    /// <summary>
    /// Creates a new BuildingManager, and initializes all BuildingTemplates.
    /// </summary>
    /// <param name="gameManager">The GameManager to which this BuildingManager belongs</param>
    /// <param name="grid">The Grid used by the GameManager</param>
    public BuildingManager(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        buildings = new List<Building>();
        templates = new List<BuildingTemplate>();
        buildingIndex = 1;
        //Load in building templates from file
        Debug.Log("Loading templates");
        string filePath = Path.Combine(Application.streamingAssetsPath, "buildings.csv");
        try
        {
            if (File.Exists(filePath))
            {
                using (StreamReader file = new StreamReader(filePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        //Read header
                        string[] header = line.Split(',');
                        int id = int.Parse(header[0]);
                        int width = int.Parse(header[1]);
                        int height = int.Parse(header[2]);
                        string prefab = header[3];
                        string name = header[4];
                        int type = int.Parse(header[5]);
                        int capacity = int.Parse(header[6]);
                        //Initialize template
                        BuildingTemplate b = new BuildingTemplate(id, name, width, height, prefab, type, capacity);
                        for (int y = 0; y < height; y++)
                        {
                            string[] row = file.ReadLine().Split(',');
                            for (int x = 0; x < width; x++)
                            {
                                b.SetCell(x, y, int.Parse(row[x]));
                            }
                        }
                        templates.Add(b);
                    }
                }
            }
            else
            {
                Debug.LogError("Could not find building template file.");
            }
        } catch(Exception e)
        {
            Debug.LogError("Failed to initialize BuildingManager!");
            Debug.LogError(e);
        }
        currentTemplate = 0;
    }
    /// <summary>
    /// Returns the current BuildingTemplate in use by the BuildingManager.
    /// </summary>
    public BuildingTemplate GetCurrentTemplate()
    {
        return templates[currentTemplate];
    }
    /// <summary>
    /// Constructs a building at the given position using the current template. The BuildingManager is not
    /// responsible for checking whether a position is clear, so use PlacementManager first!
    /// </summary>
    /// <param name="position">The grid position to place the Building at</param>
    /// <param name="template">The BuildingTemplate to use</param>
    public void ConstructBuilding(Vector2Int position, BuildingTemplate template)
    {
        Building building = new Building(buildingIndex++);
        building.SetName(template.GetName());
        building.SetBoardPos(position);
        building.SetTemplate(this.currentTemplate);
        for (int x = 0; x < template.GetWidth(); x++)
        {
            for (int y = 0; y < template.GetHeight(); y++)
            {
                if (template.GetCell(x, y) > 0)
                {
                    //Set cell to belong to new building
                    grid.SetCellBuilding(position.x + x, position.y + y, building);
                    //Add cell to building's list of contained cells
                    building.AddCell(grid.GetCell(position.x + x, position.y + y));
                }
                if (template.GetCell(x, y) == 2)//If the cell is an entrance, mark it as so
                {
                    building.AddEntrance(grid.GetCell(position.x + x, position.y + y));
                }
            }
        }
        building.SetCapacity(template.GetCapacity());
        building.SetBuildingType(template.GetBuildingType());
        building.AddGameObject(gameManager.AddNewGameObject(template.GetPrefab(), position));
        buildings.Add(building);
        Debug.Log("Created new building: " + building);
    }

    public void ConstructBuildingFromSave(BuildingSave buildSave)
    {
        this.currentTemplate = buildSave.template;
        Vector2Int position = buildSave.boardPos;
        BuildingTemplate template = templates[this.currentTemplate];
        Building building = new Building(buildingIndex++);
        building.SetName(buildSave.name);
        building.SetVisits(buildSave.visits);
        building.SetBoardPos(buildSave.boardPos);
        building.SetTemplate(buildSave.template);
        building.SetBuildingType(buildSave.type);
        for (int x = 0; x < template.GetWidth(); x++)
        {
            for (int y = 0; y < template.GetHeight(); y++)
            {
                if (template.GetCell(x, y) > 0)
                {
                    //Set cell to belong to new building
                    grid.SetCellBuilding(position.x + x, position.y + y, building);
                    //Add cell to building's list of contained cells
                    building.AddCell(grid.GetCell(position.x + x, position.y + y));
                }
                if (template.GetCell(x, y) == 2)//If the cell is an entrance, mark it as so
                {
                    building.AddEntrance(grid.GetCell(position.x + x, position.y + y));
                }
            }
        }
        buildings.Add(building);
        building.AddGameObject(gameManager.AddNewGameObject(template.GetPrefab(), position));
        Debug.Log("Created new building: " + building);
    }
    /// <summary>
    /// Demolishes the building at the given position.
    /// </summary>
    /// <param name="position"></param>
    public void DemolishBuilding(Vector2Int position)
    {
        if(grid.GetCell(position.x, position.y).IsBuilding()) {
            gameManager.RemoveGameObject(grid.GetCell(position.x, position.y).GetBuilding().GetGameObject());
            grid.GetCell(position.x, position.y).GetBuilding().Demolish();
        }
        Debug.Log("No building at (" + position.x + ", " + position.y + ") to demolish");
    }
    /// <summary>
    /// Returns a random Building of any type. If there are no Nuildings, return null;
    /// </summary>
    public Building GetRandomBuilding()
    {
        if(buildings.Count > 0)
        {
            return buildings[UnityEngine.Random.Range(0, buildings.Count)];
        }
        return null;
    }
    /// <summary>
    /// Returns the current number of Buildings
    /// </summary>
    public int GetBuildingCount() { return buildings.Count; }
    /// <summary>
    /// Increments the currently selected BuildingTemplate by the given amount.
    /// </summary>
    public void IncrementBuildingSelection(int amount)
    {
        currentTemplate = (currentTemplate + amount + templates.Count) % templates.Count;
    }

    public void SetBuildingSelection(int selection)
    {
        currentTemplate = selection;
    }

    public List<Building> GetBuildings() { return buildings; }
    public List<Building> GetBuildingsOfType(int type)
    {
        List<Building> selected = new List<Building>();
        foreach(Building building in buildings)
        {
            if(building.GetBuildingType() == type)
            {
                selected.Add(building);
            }
        }
        if(selected.Count > 0)
        {
            return selected;
        }
        return null;
    }
    public int GetBuildingCountOfType(int type)
    {
        int count = 0;
        foreach(Building building in buildings)
        {
            if(building.GetBuildingType() == type)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// Returns a random Building which matches the given type and which isn't full. Returns null if no such building can be found
    /// </summary>
    public Building GetRandomAvailableOfType(int type)
    {
        List<Building> acceptable = GetBuildingsOfType(type);
        if(acceptable == null)
        {
            throw new Exception("Could not find any buildings of type " + type);
        }
        // Iterate through the list, starting at a random point. If it loops around, then no valid building was found
        int index = UnityEngine.Random.Range(0, acceptable.Count);
        for(int i = 0; i < acceptable.Count; i++)
        {
            if (!acceptable[(index + i) % acceptable.Count].IsFull())
            {
                return acceptable[(index + i) % acceptable.Count];
            }
        }
        throw new Exception("Could not find any non-full buildings of type " + type);
    }
    /// <summary>
    /// Returns the number of available (non-full) Buildings which match the given type
    /// </summary>
    public int GetAvailableBuildingCountOfType(int type)
    {
        int count = 0;
        foreach(Building building in buildings)
        {
            if(building.GetBuildingType() == type && !building.IsFull())
            {
                count++;
            }
        }
        return count;
    }
}
