using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
/// <summary>
/// A placeable Building. Contains a list of Cells which are occupied by the Building
/// </summary>
public class Building
{
    private int id; // Unique ID for building
    private List<Cell> cells; // Cells which belong to the Building.
    private List<Cell> entrances; // Cells which are entrances to the Building. Students access the Building through entrances
    private GameObject gameObject; // GameObject representation of Building
    private int type;
    private string name;
    private int templateIndex; // The index of the template used for the buildng
    [Obsolete]
    private int visits;
    private int occupants; // The current number of occupants in the building
    private int capacity; // The maximum number of occupants the building can hold (students, cars, etc)

    private Vector2Int boardPos;
    /// <summary>
    /// Creates a new building with given ID
    /// </summary>
    public Building(int id)
    {
        this.id = id;
        cells = new List<Cell>();
        entrances = new List<Cell>();
        templateIndex = 0;
        boardPos = new Vector2Int(0, 0);
        occupants = 0;
        capacity = 0;
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
    public Cell GetRandomEntrance() { return entrances[UnityEngine.Random.Range(0, entrances.Count)]; }
    public override bool Equals(object obj)
    {
        if(obj is Building other)
        {
            return id == other.id;
        }
        return false;
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

    public void SetBuildingType(int type) { this.type = type; }
    public int GetBuildingType() { return type; }
    /// <summary>
    /// Returns a string representing the type of the building, i.e. "Residence Hall", "Lecture Hall", etc
    /// </summary>
    public string GetBuildingTypeString()
    {
        return GetBuildingTypeString(type);
    }
    public static string GetBuildingTypeString(int type)
    {
        switch (type)
        {
            case TYPE_NONE: return "None";
            case TYPE_DORMITORY: return "Residence Hall";
            case TYPE_LECTURE: return "Lecture Hall";
            case TYPE_DINING: return "Dining Hall";
            case TYPE_LIBRARY: return "Library";
            case TYPE_PARKING: return "Parking";
            case TYPE_COMMONS: return "Commons";
        }
        return "Unspecified";
    }
    /// <summary>
    /// Returns a string representing the word used to describe the capacity of the Building. Beds for residence halls, seats for lecture halls, etc
    /// </summary>
    public string GetCapacityTypeString()
    {
        switch (type)
        {
            case TYPE_NONE:
            case TYPE_COMMONS: return "Spaces";
            case TYPE_DORMITORY: return "Beds";
            case TYPE_LECTURE:
            case TYPE_DINING:
            case TYPE_LIBRARY: return "Seats";
            case TYPE_PARKING: return "Parking Spots";
        }
        return "Unspecified";
    }
    public string GetUsageString()
    {
        return GetOccupancy() + " / " + GetCapacity() + " " + GetCapacityTypeString();
    }
    /// <summary>
    /// Returns true if the building is currently at or above maximum occupancy, and false otherwise.
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        return occupants >= capacity;
    }
    public void SetCapacity(int capacity) { this.capacity = capacity; }
    public int GetCapacity() { return capacity; }
    public int GetOccupancy() { return occupants; }
    public void SetVisits(int visits) { this.visits = visits; }

    public void SetBoardPos(Vector2Int pos) { this.boardPos = pos; }
    public Vector2Int GetBoardPos() { return boardPos; }
    public void SetTemplate(int index) { this.templateIndex = index; }
    public int GetTemplate() { return templateIndex; }
    public string ToString()
    {
        return id + ", " + name + ", " + GetBuildingTypeString() + ", " + GetUsageString();
    }
    public string GetType() {
        switch(type)
        {
            case TYPE_NONE: return "none";
            case TYPE_DORMITORY: return "Dormitory";
            case TYPE_LECTURE: return "Lecture";
            case TYPE_DINING: return "Dining";
            case TYPE_LIBRARY: return "Library";
            case TYPE_PARKING: return "Parking";
            case TYPE_COMMONS: return "Commons";
            default: return "Unknown";
        };
    }
    public const int TYPE_NONE = 0;
    public const int TYPE_DORMITORY = 1;
    public const int TYPE_LECTURE = 2;
    public const int TYPE_DINING = 3;
    public const int TYPE_LIBRARY = 4;
    public const int TYPE_PARKING = 5;
    public const int TYPE_COMMONS = 6;
}
