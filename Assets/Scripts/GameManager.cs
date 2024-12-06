using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using System.IO;

public struct GameState {
    public int numStudents;
    public string selectionContext;

    public GameState(int value) {
        this.numStudents = value;
        this.selectionContext = "";
    }
}
/// <summary>
/// The GameManager is responsible for coordinating between all other managers, as well as for intializing
/// the game scene on startup.
/// </summary>

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public StudentManager studentManager;
    public BuildingManager buildingManager;
    public PlacementManager placementManager;
    public VehicleManager vehicleManager;
    public InputManager inputManager;
    public SaveManager saveManager;
    public TimeManager timeManager;
    public GameEventManager gameEventManager;
    public UILayer uiLayer;
    public ScoreSystem scoreSystem;
    private Vector2Int cursorPosition;
    public int mode;

    private int currExp;
    private double currHappiness;
    private int currLevel;
    private int currScore;


    public GameState gameState;
    private List<GameObject> gameObjects;
    public GameObject cursor;
    public Mesh cursorPrefab;

    public GameObject roadPefab;
    public GameObject busStopPrefab;
    public GameObject busVehiclePrefab;
    public GameObject crossWalkPrefab;
    public GameObject ground;

    public GameObject pathDot;
    public GameObject pathNub;
    public GameObject pathCorner;
    public GameObject pathStraight;
    public GameObject pathT;
    public GameObject pathPlus;

    public Building selectedBuilding;
    public TextMeshProUGUI buildingDisplay;

    void Start()
    {
        //Set up grid and managers
        grid = new Grid(20, 20);
        studentManager = new StudentManager(this,grid);
        buildingManager = new BuildingManager(this, grid);
        saveManager = new SaveManager(this);
        placementManager = new PlacementManager(this, buildingManager, grid);
        scoreSystem = new ScoreSystem(this, placementManager);
        vehicleManager = new VehicleManager(this, grid);
        gameEventManager = new GameEventManager(this, uiLayer);
        timeManager = new TimeManager(this, gameEventManager);
        mode = NONE;
        gameObjects = new List<GameObject>();
        gameState = new GameState(0);

        //Set up ground plane
        ground = new GameObject("Ground");
        ground.AddComponent<MeshFilter>();
        MeshRenderer renderer = ground.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.mainTexture = Resources.Load<Texture2D>("grass");
        renderer.material.mainTexture.filterMode = FilterMode.Point;
        //Define vertices
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f), //Bottom left
            new Vector3(grid.GetWidth()-0.5f, 0, -0.5f),  //Bottom right
            new Vector3(grid.GetWidth()-0.5f, 0, grid.GetHeight()-0.5f),   //Top right
            new Vector3(-0.5f, 0, grid.GetHeight()-0.5f)   //Top left
        };
        //Define triangles
        int[] triangles = new int[]
        {
            0, 2, 1, //First triangle
            0, 3, 2  //Second triangle
        };
        //Define UV
        Vector2[] uvs = {
            new Vector2(0,0), //Bottom left
            new Vector2(grid.GetWidth()/4,0), //Bottom right
            new Vector2(grid.GetWidth()/4,grid.GetHeight()/4), //Top right
            new Vector2(0,grid.GetHeight()/4)  //Top left
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        ground.GetComponent<MeshFilter>().mesh = mesh;
        //Add collider for raycasting
        MeshCollider collider = ground.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        ground.AddComponent<MeshCollider>();
    }
    void Update()
    {
        studentManager.Update();
        vehicleManager.Update();
        timeManager.Update();

        scoreSystem.SetStudentList(studentManager.GetListOfStudents());
        scoreSystem.SetBuildingList(buildingManager.GetBuildings());
        scoreSystem.UpdateExp();

        currHappiness = scoreSystem.CalOverallHappiness();
        currLevel = scoreSystem.GetLevel();
        currExp = scoreSystem.GetExp();
        currScore = scoreSystem.CalcScore();

        //Debug.Log("currHappiness: " + currHappiness + "\ncurrLevel: " + currLevel + "\ncurrExp: " + currExp + "\ncurrScore: " + currScore);
        //Update selected building UI
        if (selectedBuilding != null)
        {
            gameState.selectionContext =
                selectedBuilding.GetName() +
                "\n" + selectedBuilding.GetBuildingTypeString() +
                "\n" + selectedBuilding.GetUsageString();
        }
        else if (buildingManager.GetCurrentTemplate() != null && mode == PLACEMENT && placementManager.GetPlacementMode() == PlacementManager.BUILDING)
        {
            gameState.selectionContext = "Selected Building: " + buildingManager.GetCurrentTemplate().GetName() + "\nType: " + buildingManager.GetCurrentTemplate().GetBuildingTypeString();
        }
        else
        {
            gameState.selectionContext = "";
        }

    }
    /// <summary>
    /// Updates the position of both the world cursor GameObject, as well as the internal cursor position
    /// in grid coordinates.
    /// </summary>
    /// <param name="position"> A Vector3Int representing the position of the cursor in world space</param>
    public void UpdateCursor(Vector3Int position)
    {
        //Update cursor position
        cursor.transform.position = position;
        cursorPosition = MousePositionToGridPosition(position);
        //Update placement hologram (if eligible)
        if (mode == PLACEMENT)
        {
            switch (placementManager.GetPlacementMode())
            {
                case PlacementManager.BUILDING:
                    cursor.GetComponent<MeshFilter>().mesh = buildingManager.GetCurrentTemplate().GetPrefab().GetComponentInChildren<MeshFilter>().sharedMesh;
                    break;
            }
        }
        else
        {
            cursor.GetComponent<MeshFilter>().mesh = cursorPrefab;
        }
    }
    /// <summary>
    /// Converts the position of the cursor/raycast hit into a Vector2Int usable in the grid or for cursor
    /// GameObject positioning
    /// </summary>
    /// <param name="position"> A Vector3Int representing the position of the cursor in world space</param>
    /// <returns> Returns a Vector2Int corresponding to the 2D position of the cursor </returns>
    public Vector2Int MousePositionToGridPosition(Vector3Int position)
    {
        Vector2Int pos = new Vector2Int(position.x, position.z);
        return pos;
    }
    /// <summary>
    /// Sets the GameManager's mode to the given value
    /// 0 - GameManager.NONE - Default mode, allows inspection of buildings
    /// 1 - GameManager.PLACEMENT - Placement of buildings or pathways
    /// 2 - GameManager.DEMOLITION - Demolition of buildings or pathways
    /// </summary>
    /// <param name="val"> Desired mode.</param>
    public void SetMode(int val)
    {
        switch (val)
        {
            case NONE:
                mode = NONE;
                Debug.Log("Switched to inspection mode");
                break;
            case PLACEMENT:
                mode = PLACEMENT;
                Debug.Log("Switched to placement mode");
                break;
            case DEMOLITION:
                mode = DEMOLITION;
                Debug.Log("Switched to demolition mode");
                break;
            default:
                Debug.Log("Tried to switch to invalid mode");
                break;
        }
    }
    /// <summary>
    /// Returns the GameManager's current mode
    /// 0 - GameManager.NONE - Default mode, allows inspection of buildings
    /// 1 - GameManager.PLACEMENT - Placement of buildings or pathways
    /// 2 - GameManager.DEMOLITION - Demolition of buildings or pathways
    /// </summary>
    /// <returns> The GameManager's current mode </returns>
    public int GetMode() { return mode; }
    /// <summary>
    /// Used to alert the GameManager that the mouse was clicked, and to take action corresponding to the
    /// GameManager's current mode.
    /// </summary>
    public void MouseClicked()
    {
        switch(mode)
        {
            case NONE:
                selectedBuilding = grid.GetCell(cursorPosition.x,cursorPosition.y).GetBuilding();
                break;
            case PLACEMENT:
                placementManager.PlaceAt(cursorPosition);
                break;
            case DEMOLITION:
                placementManager.DemolishAt(cursorPosition);
                break;
        }

        if(selectedBuilding != null) {
            uiLayer.OnBuildingUIShow(ref selectedBuilding);
        }
    }
    /// <summary>
    /// Adds given GameObject to GameManager's list
    /// </summary>
    /// <param name="gameObject">Object to add to list</param>
    /// <param name="position">Starting position of the given object</param>
    /// <returns></returns>
    public GameObject AddNewGameObject(GameObject gameObject, Vector2Int position)
    {
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = new Vector3(position.x, 0, position.y);
        gameObjects.Add(obj);
        return obj;
    }
    /// <summary>
    /// Removes the given GameObject from the GameMangaer's list, and destroys it
    /// </summary>
    /// <param name="gameObject">GameObject to be removed</param>
    public void RemoveGameObject(GameObject gameObject)
    {
        gameObjects.Remove(gameObject);
        Destroy(gameObject);
    }
    /// <summary>
    /// Returns the GameManager's BuildingManager
    /// </summary>
    public BuildingManager GetBuildingManager() { return buildingManager; }
    /// <summary>
    /// Returns the GameManager's PlacementManager
    /// </summary>
    public PlacementManager GetPlacementManager() { return placementManager; }
    /// <summary>
    /// Used to cycle between non-GameManager modes, such as PlacementManager placementMode.
    /// Determined by the GameManager's current mode
    /// </summary>
    /// <param name="amount">Amount to cycle by</param>
    public GameEventManager GetEventManager() { return gameEventManager; }
    public TimeManager GetTimeManager() { return timeManager; }
    public void Cycle(int amount)
    {
        switch(mode)
        {
            case PLACEMENT:
                placementManager.Cycle(amount);
                break;
        }
    }
    /// <summary>
    /// Adds a student to the game. Student will have a random schedule.
    /// </summary>
    public void AddRandomStudent()
    {
        GameObject gameObject = studentManager.CreateStudent();
        if(gameObject != null)
        {
            gameObjects.Add(gameObject);
            gameState.numStudents++;
        }
    }
    // TODO: Create Path/RoadManager class to handle the placement and operations of roads and paths
    public void AddPath(int x, int y)
    {
        grid.GetCell(x, y).SetType(Cell.PATH);
        //Check neighbors for path status
        int arrangement = grid.GetCellPathValue(x, y);
        //Choose path model and orientation based on path arrangement
        GameObject path = GetPathModel(arrangement);        
        path.name = "path" + x + "," + y;
        path.transform.position = new Vector3(x, 0, y);
        gameObjects.Add(path);
        //Update neighboring paths
        UpdateNeighborPaths(x, y);
    }
    public GameObject GetPathModel(int arrangement)
    {
        GameObject path = null;
        switch (arrangement)
        {
            //Dot
            case 0b0000: path = Instantiate(pathDot); break;
            //Nub
            case 0b0001: path = Instantiate(pathNub); break;
            case 0b0010: path = Instantiate(pathNub); path.transform.Rotate(Vector3.up, 90); break;
            case 0b0100: path = Instantiate(pathNub); path.transform.Rotate(Vector3.up, 180); break;
            case 0b1000: path = Instantiate(pathNub); path.transform.Rotate(Vector3.up, 270); break;
            //Corner
            case 0b0011: path = Instantiate(pathCorner); break;
            case 0b0110: path = Instantiate(pathCorner); path.transform.Rotate(Vector3.up, 90); break;
            case 0b1100: path = Instantiate(pathCorner); path.transform.Rotate(Vector3.up, 180); break;
            case 0b1001: path = Instantiate(pathCorner); path.transform.Rotate(Vector3.up, 270); break;
            //Straight
            case 0b0101: path = Instantiate(pathStraight); break;
            case 0b1010: path = Instantiate(pathStraight); path.transform.Rotate(Vector3.up, 90); break;
            //T-junction
            case 0b0111: path = Instantiate(pathT); break;
            case 0b1110: path = Instantiate(pathT); path.transform.Rotate(Vector3.up, 90); break;
            case 0b1101: path = Instantiate(pathT); path.transform.Rotate(Vector3.up, 180); break;
            case 0b1011: path = Instantiate(pathT); path.transform.Rotate(Vector3.up, 270); break;
            //4-way junction
            case 0b1111: path = Instantiate(pathPlus); break;
        }
        return path;
    }
    public void UpdateNeighborPaths(int x, int y)
    {
        foreach (Cell neighbor in grid.GetCell(x, y).GetNeighbors())
        {
            if (neighbor.GetType() == Cell.PATH)
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (gameObjects[i].name == "path" + neighbor.GetX() + "," + neighbor.GetY())
                    {
                        //Destroy existing gameobject and replace with new one with correct model
                        Destroy(gameObjects[i]);
                        gameObjects[i] = GetPathModel(grid.GetCellPathValue(neighbor.GetX(), neighbor.GetY()));
                        gameObjects[i].transform.position = new Vector3(neighbor.GetX(), 0, neighbor.GetY());
                        gameObjects[i].name = "path" + neighbor.GetX() + "," + neighbor.GetY();
                    }
                }
            }
        }
    }

    public void AddRoad(int x, int y)
    {
        grid.GetCell(x, y).SetType(Cell.ROAD);
        GameObject road = Instantiate(roadPefab);
        road.name = "road" + x + "," + y;
        road.transform.position = new Vector3(x, 0, y);
        gameObjects.Add(road);
    }

    public void AddBusStop(int x, int y)
    {
        grid.GetCell(x, y).SetType(Cell.BUS_STOP);
        GameObject busStop = Instantiate(busStopPrefab);
        busStop.name = "busStop" + x + "," + y;
        busStop.transform.position = new Vector3(x, 0, y);
        gameObjects.Add(busStop);
    }
    public void AddCrossWalk(int x, int y)
    {
        grid.GetCell(x, y).SetType(Cell.CROSS_WALK);
        GameObject road = Instantiate(crossWalkPrefab);
        road.name = "crossWalk" + x + "," + y;
        road.transform.position = new Vector3(x, 0, y);
        gameObjects.Add(road);
    }

    public void AddRandomBus()
    {
        Debug.Log("About to start adding a random bus");
        GameObject bus = vehicleManager.CreateRandomBus(busVehiclePrefab);

        if (bus != null)
        {
            Debug.Log("Bus object was not NULL");
            gameObjects.Add(bus);
        }
    }

    public void DemolishPath(Vector2Int position)
    {
        //Reset cell type and delete gameObject
        Debug.Log("demolishing " + position.x + ", " + position.y);
        grid.GetCell(position.x, position.y).SetType(Cell.EMPTY);
        Debug.Log(gameObjects.Count + " objects");
        for(int i = 0; i < gameObjects.Count; i++)
        {
                Debug.Log(gameObjects[i].name);
            if(gameObjects[i].name == ("path" + position.x + "," + position.y))
            {
                UpdateNeighborPaths(position.x, position.y);
                Debug.Log("demolishing");
                Destroy(gameObjects[i]);
                break;
            }
        }
        gameObjects.RemoveAll(obj => obj.name == ("path" + position.x + "," + position.y));
    }

    public void DemolishRoad(Vector2Int position)
    {
        //Reset cell type and delete gameObject
        grid.GetCell(position.x, position.y).SetType(Cell.EMPTY);
        foreach (GameObject obj in gameObjects)
        {
            if (obj.name == ("road" + position.x + "," + position.y))
            {
                Debug.Log("demolishing");
                Destroy(obj);
            }
        }
        gameObjects.RemoveAll(obj => obj.name == ("road" + position.x + "," + position.y));
    }

    public void DemolishBusStop(Vector2Int position)
    {
        //Reset cell type and delete gameObject
        grid.GetCell(position.x, position.y).SetType(Cell.EMPTY);
        foreach (GameObject obj in gameObjects)
        {
            if (obj.name == ("busStop" + position.x + "," + position.y))
            {
                Debug.Log("demolishing");
                Destroy(obj);
            }
        }
        gameObjects.RemoveAll(obj => obj.name == ("busStop" + position.x + "," + position.y));
    }

    public void DemolishCrossWalk(Vector2Int position)
    {
        //Reset cell type and delete gameObject
        grid.GetCell(position.x, position.y).SetType(Cell.EMPTY);
        foreach (GameObject obj in gameObjects)
        {
            if (obj.name == ("crossWalk" + position.x + "," + position.y))
            {
                Debug.Log("demolishing");
                Destroy(obj);
            }
        }
        gameObjects.RemoveAll(obj => obj.name == ("crossWalk" + position.x + "," + position.y));
    }
    /// <summary>
    /// Used to clear the GameManager's currently selected building
    /// </summary>
    public void ClearSelectedBuilding()
    {
        selectedBuilding = null;
    }
    public GameState GetGameState() { return gameState; }
    public List<GameObject> GetGameObjects() { return gameObjects; }
    public Building GetSelectedBuilding() { return selectedBuilding; }
    public void SaveGame()
    {
        saveManager.SerializeToSaveFile();
    }

    public void LoadGame()
    {
        // reset the scene first
        //ResetGame();

        saveManager.LoadFromSaveFile();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(1);
    }
    public const int NONE = 0;
    public const int PLACEMENT = 1;
    public const int DEMOLITION = 2;
}
