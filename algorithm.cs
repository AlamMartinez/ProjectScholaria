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
	private bool isOnBicycle; // Flag for bicycle status
	private GameObject flagObject; // Reference to the flag GameObject

	public Student(int id, bool isOnBicycle = false)
	{
		this.id = id;
		this.isOnBicycle = isOnBicycle; // Initialize bicycle flag
		travel = 0;
	}

	/// <summary>
	/// Populates the path list with Cells which represent a path going between the Student's
	/// current position, and the closest entrance belonging to the Student's destination 
	/// building. Uses Dijkstra's algorithm to compute the path.
	/// </summary>
	/// <param name="grid">The Grid used to calculate the path</param>
	public void CalculatePath(Grid grid)
	{
		Debug.Log("Student " + id + " is finding a path from " + position + " to building " + destination.GetID());
		path = new List<Cell>();

		Dictionary<Cell, int> distMap = new Dictionary<Cell, int>();
		Dictionary<Cell, Cell> parents = new Dictionary<Cell, Cell>();
		List<Cell> queue = new List<Cell>();

		distMap[position] = 0;
		parents[position] = null;
		queue.Add(position);

		while (queue.Count > 0)
		{
			queue.Sort((c1, c2) => distMap[c1].CompareTo(distMap[c2]));
			Cell current = queue[0];
			queue.RemoveAt(0);

			if (current.IsBuilding() && current.GetBuilding().GetID() == destination.GetID())
			{
				while (current != null)
				{
					path.Add(current);
					current = parents[current];
				}
				path.Reverse();
				break;
			}

			foreach (Cell neighbor in current.GetNeighbors())
			{
				if (current.IsBuilding() == neighbor.IsBuilding() || current.IsEntrance() || neighbor.IsEntrance())
				{
					int dist = distMap[current] + neighbor.GetWeight();
					if (current.IsBuilding() != neighbor.IsBuilding())
					{
						dist += 5;
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
		if (path.Count == 0)
		{
			Debug.LogError("Student " + id + " could not find path from " + position + " to building " + destination.GetID());
		}
		else
		{
			PrintPathfindingMap(grid, distMap);
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
	public bool Update()
	{
		if (gameObject != null && path.Count > 0)
		{
			gameObject.transform.position = new Vector3(
				Mathf.Lerp(position.GetX(), path[0].GetX(), travel),
				0,
				Mathf.Lerp(position.GetY(), path[0].GetY(), travel)
			);
		}

		if (path.Count > 0)
		{
			// Increase speed for students on bicycles
			float speedMultiplier = isOnBicycle ? 1.5f : 1.0f;
			travel += Time.deltaTime * position.GetTraversalSpeed() * speedMultiplier;

			if (travel > 1)
			{
				if (path.Count > 0 && position.IsEntrance() && path[0].GetType() != Cell.BUILDING)
				{
					position.GetBuilding().AddVisit();
				}
				travel %= 1;
				position = path[0];
				path.RemoveAt(0);
			}
			return true;
		}
		return false;
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
	public void SetPosition(Cell cell)
	{
		travel = 0;
		position = cell;
	}

	/// <summary>
	/// Sets the Student's GameObject and adds a flag if the student is on a bicycle
	/// </summary>
	public void SetGameObject(GameObject gameObject)
	{
		this.gameObject = gameObject;

		if (isOnBicycle)
		{
			// Instantiate flag above the student's head
			flagObject = GameObject.Instantiate(Resources.Load<GameObject>("FlagPrefab"));
			flagObject.transform.SetParent(gameObject.transform);
			flagObject.transform.localPosition = new Vector3(0, 2, 0); // Adjust Y value to position above head
		}
	}

	public void Remove()
	{
		Object.Destroy(gameObject);
	}

	public int GetID() { return id; }

	/// <summary>
	/// Prints a map of the Student's last pathfinding attempt. Values indicate the path
	/// distance from the given cell to the Student's current position, represented by zero.
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
}
