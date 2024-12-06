using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A template for a Building. Uses a 2D array of integers to represent the Building's footprint, with
/// 0's representing empty space, 1's representing structures, and 2's representing entrances to the
/// building. Also includes a GameObject prefab, which is placed in the world alongside the creation of
/// the Building object.
/// </summary>
public class BuildingTemplate
{
    private int id;
    private string name;
    private int type;
    private int capacity;
    private int[,] cells;
    private GameObject prefab;

    public BuildingTemplate(int id, string name, int width, int height, string prefabName, int type, int capacity) 
    {
        this.id = id;
        cells = new int[width, height];
        this.name = name;
        this.type = type;
        this.capacity = capacity;
        prefab = Resources.Load<GameObject>("Buildings/"+ prefabName);
        Debug.Log("Created new building template: " + name + ", " + width + " x " + height + ", ID: " + id);
    }
    public void SetCell(int x, int y, int val) { cells[x, y] = val; }
    public int GetCell(int x, int y) { return cells[x, y]; }
    public int GetWidth() { return cells.GetLength(0); }
    public int GetHeight() { return cells.GetLength(1); }
    //public void SetName(string name) { this.name = name; }
    public string GetName() { return this.name; }
    public GameObject GetPrefab() { return prefab; }
    public int GetBuildingType() { return type; }
    public string GetBuildingTypeString() { return Building.GetBuildingTypeString(type); }
    public int GetCapacity() { return capacity; }
    // TODO: function to return building type
}
