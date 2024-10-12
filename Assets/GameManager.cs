using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Grid grid;
    public StudentManager studentManager;
    public BuildingManager buildingManager;
    public PlacementManager placementManager;
    public InputManager inputManager;
    
    private Vector2Int cursorPosition;
    private int mode;

    private List<GameObject> gameObjects;
    public GameObject cursor;
    public Mesh cursorPrefab;
    public GameObject pathPrefab;
    public GameObject ground;

    public Building selectedBuilding;
    public TextMeshProUGUI buildingDisplay;
    public TextMeshProUGUI modeDisplay;

    void Start()
    {
        //Set up grid and managers
        grid = new Grid(20, 20);
        studentManager = new StudentManager(this,grid);
        buildingManager = new BuildingManager(this, grid);
        placementManager = new PlacementManager(this, buildingManager, grid);
        mode = NONE;
        gameObjects = new List<GameObject>();

        //Set up ground plane
        ground = new GameObject("Ground");
        ground.AddComponent<MeshFilter>();
        MeshRenderer renderer = ground.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        Mesh mesh = new Mesh();
        //Define vertices
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
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        ground.GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider collider = ground.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        ground.AddComponent<MeshCollider>();
        
    }
    void Update()
    {
        studentManager.Update();
        //Update selected building UI
        if(selectedBuilding != null)
        {
            buildingDisplay.text = "Selected Building: " + selectedBuilding.GetName() + "\nDesignation: " + selectedBuilding.GetType() + "\nVisits: " + selectedBuilding.GetVisits();
        }
        else if (buildingManager.GetCurrentTemplate() != null && mode == PLACEMENT && placementManager.GetPlacementMode() == PlacementManager.BUILDING)
        {
            buildingDisplay.text = "Selected Building: " + buildingManager.GetCurrentTemplate().GetName();
        }
        else
        {
            buildingDisplay.text = "";
        }
        //Update mode UI
        switch(mode)
        {
            case NONE:
                modeDisplay.text = "Mode: Overview";
                break;
            case PLACEMENT: 
                switch(placementManager.GetPlacementMode())
                {
                    case PlacementManager.BUILDING:
                        modeDisplay.text = "Mode: Placing Buildings";
                        break;
                    case PlacementManager.PATHING:
                        modeDisplay.text = "Mode: Placing Pathways";
                        break;
                }
                break;
            case DEMOLITION:
                modeDisplay.text = "Mode: Demolition";
                break;
        }
    }
    //Update cursor information
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
    //Convert 3D raycast position into 2D position
    public Vector2Int MousePositionToGridPosition(Vector3Int position)
    {
        Vector2Int pos = new Vector2Int(position.x, position.z);
        return pos;
    }
    //Change current mode
    public void SetMode(int val)
    {
        mode = val;
    }
    public int GetMode() { return mode; }
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
    }
    //Adds gameObject to the scene at position
    public GameObject AddNewGameObject(GameObject gameObject, Vector2Int position)
    {
        GameObject obj = Instantiate(gameObject);
        obj.transform.position = new Vector3(position.x, 0, position.y);
        gameObjects.Add(obj);
        return obj;
    }
    //Removes the gameObject from the manager's list, then destroys it
    public void RemoveGameObject(GameObject gameObject)
    {
        gameObjects.Remove(gameObject);
        Destroy(gameObject);
    }
    public BuildingManager GetBuildingManager() { return buildingManager; }
    public PlacementManager GetPlacementManager() { return placementManager; }
    public void Cycle(int amount)
    {
        switch(mode)
        {
            case PLACEMENT:
                placementManager.Cycle(amount);
                break;
        }
    }
    public void AddRandomStudent()
    {
        GameObject gameObject = studentManager.CreateRandomStudent();
        if(gameObject != null)
        {
            gameObjects.Add(gameObject);
        }
    }
    public void AddPath(int x, int y)
    {
        grid.GetCell(x, y).SetType(Cell.PATH);
        GameObject path = Instantiate(pathPrefab);
        path.name = "path" + x + "," + y;
        path.transform.position = new Vector3(x, 0, y);
        gameObjects.Add(path);
    }
    public void DemolishPath(Vector2Int position)
    {
        //Reset cell type and delete gameObject
        grid.GetCell(position.x, position.y).SetType(Cell.EMPTY);
        foreach(GameObject obj in gameObjects)
        {
            if(obj.name == ("path" + position.x + "," + position.y))
            {
                Debug.Log("demolishing");
                Destroy(obj);
            }
        }
        gameObjects.RemoveAll(obj => obj.name == ("path" + position.x + "," + position.y));
    }
    public void ClearSelectedBuilding()
    {
        selectedBuilding = null;
    }
    public const int NONE = 0;
    public const int PLACEMENT = 1;
    public const int DEMOLITION = 2;
}
