using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The building block of the Grid, Cells represent individual tiles within the Grid, upon
/// which Buildings and paths can be placed.
/// </summary>
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
    /// <summary>
    /// Sets the Cell's neighbors. Two Cells are neigbhors if they share an edge
    /// </summary>
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
    /// <summary>
    /// Returns true if this Cell belongs to a Building, and false otherwise
    /// </summary>
    public bool IsBuilding()
    {
        return type == BUILDING;
    }
    /// <summary>
    /// Returns true if this Cell belongs to a Building, and is an entrance for that building, 
    /// and false otherwise
    /// </summary>
    public bool IsEntrance()
    {
        return building != null && building.IsEntrance(this);
    }
    /// <summary>
    /// Returns the weight of the Cell for pathfinding
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Returns the speed at which a student traverses this Cell.
    /// </summary>
    /// <returns></returns>
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
