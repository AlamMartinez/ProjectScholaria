using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BuildingManager
{
    private GameManager gameManager;
    private Grid grid;
    private List<Building> buildings;
    private List<BuildingTemplate> templates;
    private int currentTemplate;
    private int buildingIndex;

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
        if(File.Exists(filePath))
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                    //Read header
                    string[] header = line.Split(',');
                    int id = int.Parse(header[0]);
                    int width = int.Parse(header[1]);
                    int height = int.Parse(header[2]);
                    string prefab = header[3];
                    //Initialize template
                    BuildingTemplate b = new BuildingTemplate(id,width,height,prefab);
                    for(int y = 0; y < height; y++)
                    {
                        string[] row = file.ReadLine().Split(',');
                        for(int x = 0; x < width; x++)
                        {
                            b.SetCell(x, y, int.Parse(row[x]));
                        }
                    }
                    b.SetName(prefab);
                    templates.Add(b);
                }
            }
        }
        else
        {
            Debug.LogError("Could not find building template file.");
        }
        currentTemplate = 0;
    }
    public BuildingTemplate GetCurrentTemplate()
    {
        return templates[currentTemplate];
    }
    public void ConstructBuilding(Vector2Int position, BuildingTemplate template)
    {
        Building building = new Building(buildingIndex++);
        building.SetName(template.GetName());
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
    public void DemolishBuilding(Vector2Int position)
    {
        gameManager.RemoveGameObject(grid.GetCell(position.x, position.y).GetBuilding().GetGameObject());
        grid.GetCell(position.x, position.y).GetBuilding().Demolish();
    }
    //Returns a random building of any type
    public Building GetRandomBuilding()
    {
        return buildings[Random.Range(0, buildings.Count)];
    }
    //Returns the current number of buildings
    public int GetBuildingCount() { return buildings.Count; }
    //Changes the currently selected building template
    public void IncrementBuildingSelection(int amount)
    {
        currentTemplate = (currentTemplate + amount + templates.Count) % templates.Count;
    }
}
