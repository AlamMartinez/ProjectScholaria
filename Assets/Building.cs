using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Building
{
    private int id;
    private List<Cell> cells;
    private List<Cell> entrances;
    private GameObject gameObject;
    private int type;
    private string name;
    private int visits;

    public Building(int id)
    {
        this.id = id;
        cells = new List<Cell>();
        entrances = new List<Cell>();
        type = Random.Range(1, 4);
        visits = 0;
    }
    public void AddCell(Cell cell)
    {
        cells.Add(cell);
    }
    public void AddEntrance(Cell cell)
    {
        entrances.Add(cell);
    }
    //Returns true if cell is an entrance to the building
    public bool IsEntrance(Cell cell)
    {
        return entrances.Contains(cell);
    }
    //Demolishes a building. Sets all cells to empty and removes their building, then removes them from lists
    public void Demolish()
    {
        foreach (Cell cell in cells)
        {
            cell.RemoveBuilding();
        }
        cells.Clear();
        entrances.Clear();
    }
    //Sets the building's gameObject in the scene;
    public void AddGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    //Return building's gameObject
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public int GetID() { return id; }
    //Return a random entrance of the building, used for student pathfinding
    public Cell GetRandomEntrance() { return entrances[Random.Range(0, entrances.Count)]; }
    public override bool Equals(object obj)
    {
        if(obj is Building other)
        {
            return id == other.id;
        }
        return false;
    }
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
