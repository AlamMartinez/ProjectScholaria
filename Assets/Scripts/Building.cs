using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
/// <summary>
/// A placeable Building. Contains a list of Cells which are occupied by the Building
/// </summary>
public class Building
{
    private int id;
    private List<Cell> cells;
    private List<Cell> entrances;
    private GameObject gameObject;
    private int type;
    private string name;
    private int visits;
    /// <summary>
    /// Creates a new building with given ID
    /// </summary>
    public Building(int id)
    {
        this.id = id;
        cells = new List<Cell>();
        entrances = new List<Cell>();
        type = Random.Range(1, 4);
        visits = 0;
    }
    /// <summary>
    /// Adds the given Cell to the Building's list of included cells
    /// </summary>
    public void AddCell(Cell cell)
    {
        cells.Add(cell);
    }
    /// <summary>
    /// Adds the given Cell to the Building's list of entrances
    /// </summary>
    public void AddEntrance(Cell cell)
    {
        entrances.Add(cell);
    }
    /// <summary>
    /// Returns true if the given Cell is an entrance to this Building, and false otherwise
    /// </summary>
    public bool IsEntrance(Cell cell)
    {
        return entrances.Contains(cell);
    }
    /// <summary>
    /// Removes all Cells from this Building's lists, and clears the set builing for all contained Cells
    /// </summary>
    public void Demolish()
    {
        foreach (Cell cell in cells)
        {
            cell.RemoveBuilding();
        }
        cells.Clear();
        entrances.Clear();
    }
    /// <summary>
    /// Sets the Building's GameObject representation
    /// </summary>
    public void AddGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    /// <summary>
    /// Gets the Building's GameObject representation
    /// </summary>
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public int GetID() { return id; }
    /// <summary>
    /// Returns a random entrance included within the Building. 
    /// </summary>
    public Cell GetRandomEntrance() { return entrances[Random.Range(0, entrances.Count)]; }
    public override bool Equals(object obj)
    {
        if(obj is Building other)
        {
            return id == other.id;
        }
        return false;
    }
    /// <summary>
    /// Returns the string representation of this Building's type.
    /// </summary>
    public string GetType()
    {
        switch(type)
        {
            case TYPE_NONE:return "None";
            case TYPE_DORMITORY:return "Residence Hall";
            case TYPE_LECTURE:return "Lecture Hall";
            case TYPE_DINING:return "Dining Hall";
        }
        return "Unspecified";
    }
    /// <summary>
    /// Sets this Building's name. Can be different than its prefab name.
    /// </summary>
    public void SetName(string name) { this.name = name; }
    //Returns a string containing the name of the building in title case
    public string GetName()
    {
        return name;
    }
    //Adds a visit to the buildings
    public void AddVisit()
    {
        visits++;
    }
    //Retrieves the number of times the building has been visited
    public int GetVisits()
    {
        return visits;
    }
    public const int TYPE_NONE = 0;
    public const int TYPE_DORMITORY = 1;
    public const int TYPE_LECTURE = 2;
    public const int TYPE_DINING = 3;
}
