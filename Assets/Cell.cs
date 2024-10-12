using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private int type;
    private int x;
    private int y;
    private List<Cell> neighbors;
    private Building building;

    public Cell(int x, int y, int type)
    {
        this.x = x;
        this.y = y;
        this.type = EMPTY;
    }
    public void SetNeighbors(List<Cell> neighbors)
    {
        this.neighbors = neighbors;
    }
    public int GetType() { return type; }
    public void SetType(int type) { this.type = type; }
    public int[] GetPosition() { return new int[] { x, y }; }
    public int GetX() { return x; }
    public int GetY() { return y; }
    public List<Cell> GetNeighbors() { return neighbors; }
    //Sets cell as belonging to given building, and marks it as occupied
    public void SetBuilding(Building building)
    {
        this.building = building;
        type = BUILDING;
    }
    public Building GetBuilding()
    {
        return building;
    }
    public void RemoveBuilding()
    {
        building = null;
        type = EMPTY;
    }
    //Returns true if this cell belongs to a building
    public bool IsBuilding()
    {
        return type == BUILDING;
    }
    //Returns true if this cell is an entrance to its building
    public bool IsEntrance()
    {
        return building != null && building.IsEntrance(this);
    }
    //Returns the pathfinding weight of the cell
    public int GetWeight()
    {
        switch(type)
        {
            case EMPTY: return 3;
            case PATH:
            case BUILDING: return 1;
            default: return 5;
        }
    }
    public override string ToString()
    {
        return "Cell (" + x + ", " + y + "), type: " + type + ", building: " + ((building == null) ? ("null") : ("" + building.GetID()));
    }
    public override bool Equals(object obj)
    {
        if(obj is Cell other)
        {
            return x == other.x && y == other.y;
        }
        return false;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + x.GetHashCode();
            hash = hash * 31 + y.GetHashCode();
            return hash;
        }
    }
    //The speed at which a student walks accross the cell
    public float GetTraversalSpeed()
    {
        if(type == EMPTY)
        {
            return 0.5f;
        }
        return 1;
    }
    public const int EMPTY = 0;
    public const int PATH = 1;
    public const int BUILDING = 2;
}
