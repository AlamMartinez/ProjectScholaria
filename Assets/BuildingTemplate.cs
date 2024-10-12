using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTemplate
{
    private int id;
    private string name;
    private int[,] cells;
    private GameObject prefab;

    public BuildingTemplate(int id, int width, int height, string name) 
    {
        this.id = id;
        cells = new int[width, height];
        this.name = name;
        prefab = Resources.Load<GameObject>(name);
        Debug.Log("Created new building template: " + name + ", " + width + " x " + height + ", ID: " + id);
    }
    public void SetCell(int x, int y, int val) { cells[x, y] = val; }
    public int GetCell(int x, int y) { return cells[x, y]; }
    public int GetWidth() { return cells.GetLength(0); }
    public int GetHeight() { return cells.GetLength(1); }
    public void SetName(string name) { this.name = name; }
    public string GetName() { return this.name; }
    public GameObject GetPrefab() { return prefab; }
}
