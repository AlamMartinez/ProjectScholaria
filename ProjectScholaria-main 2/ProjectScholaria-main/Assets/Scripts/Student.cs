using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Student agent
/// </summary>
public class Student
{
    private int id;
    private Cell position;
    private Building destination;
    private List<Cell> path;

    private float travel;
    private GameObject gameObject;

    private int happiness;
    private int scheduleIndex;
    private List<Building> schedule;

    public Student(int id)
    {
        this.id = id;
        travel = 0;
        schedule = new List<Building>();
        scheduleIndex = 0;
    }
    /// <summary>
    /// Generates a new schedule for the student based on current building availability
    /// </summary>
    public void GenerateSchedule(BuildingManager bm)
    {
        try
        {
            // Find a home dorm for the student. Will serve as their starting and ending point
            schedule.Add(bm.GetRandomAvailableOfType(Building.TYPE_DORMITORY));
            // Find classes for the student.
            int maxClasses = bm.GetAvailableBuildingCountOfType(Building.TYPE_LECTURE);
            if(maxClasses <= 0)
            {
                throw new Exception("Could not find any non-full classes.");
            }
            int classCount = UnityEngine.Random.Range(1, maxClasses);
            for(int i = 0; i < classCount; i++)
            {
                // TODO: Can be optimized
                Building destination;
                do
                {
                    destination = bm.GetRandomAvailableOfType(Building.TYPE_LECTURE);
                } while (schedule.Contains(destination));
                schedule.Add(destination);
            }
            // Send student home to dorm at end of schedule
            schedule.Add(schedule[0]);
            // Set student's starting positon to a random entrance of their dorm
            position = schedule[0].GetRandomEntrance();
            PrintSchedule();
        } catch(Exception e)
        {
            Debug.LogError("Could not generate a schedule for student " + id + "; " + e);
        }
    }
    public bool HasSchedule()
    {
        return schedule.Count > 0;
    }
    /// <summary>
    /// Populates the path list with Cells which represent a path going between the Student's
    /// current postion, and the closest entrance belonging to the Student's destination 
    /// building. Uses Dijkstra's algorithm to compute the path.
    /// </summary>
    /// <param name="grid">The Grid used to calculate the path</param>
    public void CalculatePath(Grid grid)
    {
        // Set destination to current schedule building
        if(destination == null && scheduleIndex + 1 < schedule.Count)
        {
            destination = schedule[scheduleIndex + 1];
        }
        Debug.Log("Student " + id + " is finding a path from " + position + " to building " + destination.GetID());
        path = new List<Cell>();

        Dictionary<Cell, int> distMap = new Dictionary<Cell, int>();
        Dictionary<Cell, Cell> parents = new Dictionary<Cell, Cell>();
        List<Cell> queue = new List<Cell>();

        distMap[position] = 0;
        parents[position] = null;
        queue.Add(position);

        while(queue.Count > 0)
        {
            //Sort queue by distance
            queue.Sort((c1, c2) => distMap[c1].CompareTo(distMap[c2]));
            //Poll from queue
            Cell current = queue[0];
            queue.RemoveAt(0);
            //Check if current cell is an entrance in the destination building
            if(current.IsBuilding() && current.GetBuilding().GetID() == destination.GetID())
            {
                //If it is, determine the path to the building using the parent dictionary
                while(current != null)
                {
                    path.Add(current);
                    current = parents[current];
                }
                //Reverse path, so list is from position to destination
                path.Reverse();
                break;
            }
            //Otherwise, check all neighbors
            foreach (Cell neighbor in current.GetNeighbors())
            {
                if (neighbor.IsRoad())
                {
                    continue;
                }

                //Only allow students to enter/exit buildings through entrances
                if (current.IsBuilding() == neighbor.IsBuilding() || current.IsEntrance() || neighbor.IsEntrance())
                {
                    //Determine weight
                    int dist = distMap[current] + neighbor.GetWeight();
                    //If moving into/out from a building, add an additional penalty
                    if (current.IsBuilding() != neighbor.IsBuilding())
                    {
                        dist += 50;
                    }
                    if (dist < (distMap.ContainsKey(neighbor) ? distMap[neighbor] : int.MaxValue))
                    {
                        distMap[neighbor] = dist;
                        parents[neighbor] = current;
                        queue.Remove(neighbor);
                        queue.Add(neighbor);
                    }
                }
            }
        }
        //Check that path was formed
        if(path.Count == 0)
        {
            Debug.LogError("Student " + id + " could not find path from " + position + " to building " + destination.GetID());
        }
        else
        {
            //PrintPathfindingMap(grid,distMap);
        }
    }
    /// <summary>
    /// Returns true if the Student has a valid path, and false otherwise.
    /// </summary>
    public bool HasPath()
    {
        return path != null && path.Count > 0;
    }
    /// <summary>
    /// Update the Student's position along the path
    /// </summary>
    /// <returns></returns>
    public void Update(Grid grid)
    {
        if(path == null)
        {
            CalculatePath(grid);
        }
        if(gameObject != null && path.Count > 0)
        {
            gameObject.transform.position = new Vector3(
                Mathf.Lerp(position.GetX(), path[0].GetX(),travel), 
                0,
                Mathf.Lerp(position.GetY(), path[0].GetY(), travel)
            );
            
        }
        if(path.Count > 0)
        {
            travel += Time.deltaTime * position.GetTraversalSpeed();
            if(travel > 1)
            {
                //Update building visit statistics
                if (path.Count > 0 && position.IsEntrance() && path[0].GetType() != Cell.BUILDING)
                {
                    position.GetBuilding().AddVisit();
                }
                //If student has reached the end of the current cell, remove it and get the next one
                travel %= 1;
                position = path[0];
                path.RemoveAt(0);
            }
        }
        else
        {
            if(scheduleIndex + 1 < schedule.Count)
            {
                destination = schedule[++scheduleIndex];
                CalculatePath(grid);
            }
            else
            {
                Debug.Log("Student " + id + " has reached the end of their schedule.");
            }
        }
    }
    /// <summary>
    /// Returns the current Cell the Student occupies
    /// </summary>
    public Cell GetPositionCell() { return position; }
    /// <summary>
    /// Sets the Student's destination building. Will not update pathfinding.
    /// </summary>
    public void SetDestination(Building destination)
    {
        this.destination = destination;
    }
    /// <summary>
    /// Sets the Student's current position Cell and resets traversal.
    /// </summary>
    //TODO: Check behavior when changing position while Student is moving.
    public void SetPosition(Cell cell)
    {
        travel = 0;
        position = cell;
    }
    /// <summary>
    /// Set's the Student's GameObject
    /// </summary>
    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public void Remove()
    {
        UnityEngine.Object.Destroy(gameObject);
    }
    public int GetID() { return id; }
    /// <summary>
    /// Prints a map of the Student's last pathfinding attempt. Values indicate the path
    /// distance from the given cell to the Student's current positon, represented by zero.
    /// </summary>
    public void PrintPathfindingMap(Grid grid, Dictionary<Cell, int> distMap)
    {
        Debug.Log("Student " + id + "'s path from " + position + " to building " + destination.GetID());
        string output = "";
        for (int y = 0; y < grid.GetHeight(); y++)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                if (distMap.ContainsKey(grid.GetCell(x, y)))
                {
                    output += distMap[grid.GetCell(x, y)].ToString().PadLeft(4) + ",";
                }
                else
                {
                    output += "    ,";
                }
            }
            output += "\n";
        }
        Debug.Log(output);
    }
    public void PrintSchedule()
    {
        Debug.Log("Student " + id + "'s schedule:");
        string output = "";
        foreach(Building b in schedule) {
            output += b.ToString() + "\n";
        }
        Debug.Log(output);
    }

    public int GetHappiness()
    {
        return happiness;
    }

    public void IncreaseHappiness(int amount)
    {
        happiness = Mathf.Min(happiness + amount, 100);
    }

    public void DecreaseHappiness(int amount)
    {
        happiness = Mathf.Max(happiness - amount, 0);
    }
}
