using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicles
{
    private int id;
    private Cell position;
    private Building destination;
    private List<Cell> path;
    private float travel;
    private GameObject gameObject;
    private char vehicleType;
    private int studentCapacity;
    private int currCapacity;
    private int busStopIndex;

    public Vehicles(int id, char vechicleType)
    {
        this.id = id;
        this.vehicleType = vechicleType;
        travel = 0;
        currCapacity = 0;
        busStopIndex = 1;
    }

    public void SetStudentCapacity(char vehicleType)
    {
        switch (vehicleType)
        {
            case 'b':
                studentCapacity = 20;
                break;
            case 'c':
                studentCapacity = 4;
                break;
        }
    }

    public void SetStudentCapacity(int studentCapacity) { this.studentCapacity = studentCapacity; }

    public void CalculateRoad(Grid grid, List<Cell> busStops)
    {
        Debug.Log("Bus: " + id + " is finding the route");
        path = new List<Cell>();

        Dictionary<Cell, int> distMap = new Dictionary<Cell, int>();
        Dictionary<Cell, Cell> parents = new Dictionary<Cell, Cell>();
        List<Cell> queue = new List<Cell>();

        distMap[position] = 0;
        parents[position] = null;
        queue.Add(position);

        while (queue.Count > 0)
        {
            queue.Sort((cell1, cell2) => distMap[cell1].CompareTo(distMap[cell2]));
            Cell currentCell = queue[0];
            queue.RemoveAt(0);
            //Debug.Log("CurrCELL is at : " + currentCell.GetX() + ", " + currentCell.GetY());
            //Debug.Log("Bus stop 0 is at : " + busStops[0].GetX() + ", " + busStops[0].GetY());
           // Debug.Log("Bus stop 1 is at : " + busStops[1].GetX() + ", " + busStops[1].GetY());
            if (currentCell.IsBusStop() && (currentCell.GetX() == busStops[busStopIndex].GetX()
                    && currentCell.GetY() == busStops[busStopIndex].GetY()))
            {
                while (currentCell != null)
                {
                    path.Add(currentCell);
                    //Debug.Log("adding currCELL [" + currentCell.GetX() + ", " + currentCell.GetY()
                    //    + "] to path");
                    currentCell = parents[currentCell];
                }

                path.Reverse();
                break;
            }

            foreach (Cell neighborCell in currentCell.GetNeighbors())
            {
                if (neighborCell.IsRoad() || neighborCell.IsBusStop())
                {
                    int dist = distMap[currentCell] + neighborCell.GetWeight();
                    //Debug.Log("currNE [" + neighborCell.GetX() + ", " + neighborCell.GetY()
                    //    + "]  and curr Cell [" + currentCell.GetX() + ", " + currentCell.GetY() + "]");
                    
                    
                    if (dist < (distMap.ContainsKey(neighborCell) ? distMap[neighborCell] : int.MaxValue))
                    {
                        //Debug.Log("currNEIGHBOR inside thingy");
                        distMap[neighborCell] = dist;
                        parents[neighborCell] = currentCell;
                        queue.Remove(neighborCell);
                        queue.Add(neighborCell);
                    }
                }
            }

        }

        busStopIndex++;
        busStopIndex = busStopIndex % busStops.Count;
        if (path.Count == 0)
        {
            Debug.Log("bus " + id + " could not find route");
        }
        else
        {
            Debug.Log("bus " + id + " ROUTE MADE");
        }

    }
    
    public bool HasPath()
    {
        return (path != null && path.Count > 0);
    }

    public Cell GetPosition() { return position; }

    public void SetPosition(Cell cell)
    {
        travel = 0;
        position = cell;
    }
    public void SetGameObject(GameObject gameObject) { this.gameObject = gameObject; }

    public void Remove() { Object.Destroy(gameObject); }

    public int GetID() { return id; }

    public int GetCurrCapacity() { return currCapacity; }

    public bool AddStudent()
    {
        if (currCapacity <= studentCapacity)
        {
            currCapacity++;
            Debug.Log("Added student to vehicle of ID: " + id + "\n The vehicle now has: " + currCapacity + " students");
            return true;
        }
        else
        {
            Debug.Log("No Student added to vehicle of ID: " + id + "\n The vehicle now has: " + currCapacity + " students");
            return false;
        }
    }
    public void RemoveStudent()
    {
        studentCapacity--;
        if (studentCapacity < 0)
            studentCapacity = 0;
        Debug.Log("Student has been Removed from vehcile of ID " + id + "\n The vehicle now has: " + currCapacity + " students");
    }

    public bool Update()
    {
        Debug.Log("Bus: " + id + " has a path of [" + path.Count + "]");
        if (gameObject != null && path.Count > 0)
        {
            Debug.Log("currPosi [" + position.GetX() + ", " + position.GetY() + "]" +
                " curr path[0] is [" + path[0].GetX() + ", " + position.GetY() + "]");
            gameObject.transform.position = new Vector3(
                Mathf.Lerp(position.GetX(), path[0].GetX(), travel),
                0,
                Mathf.Lerp(position.GetY(), path[0].GetY(), travel)
            );

        }
        if (path.Count > 0)
        {
            travel += Time.deltaTime * position.GetTraversalSpeed() * 3;
            if (travel > 1)
            {
                //If student has reached the end of the current cell, remove it and get the next one
                travel %= 1;
                position = path[0];
                path.RemoveAt(0);

            }
            return true;
        }
        return false;
    }

}
