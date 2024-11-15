using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the creation, deletion, and pathfinding of Students.
/// </summary>
public class StudentManager
{
    private Grid grid;
    private GameManager gameManager;
    private List<Student> students;
    private int studentIndex;

    public StudentManager(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        students = new List<Student>();
    }
    /// <summary>
    /// Updates the position and, if elligble, pathfinding of every Student.
    /// </summary>
    public void Update()
    {
        Debug.Log("updating students...");
        foreach (Student student in students)
        {
            Debug.Log(student == null);
            //If student does not have a path, find a new one.
            Debug.Log("Updating student " + student.GetID());
            if(!student.HasPath())
            {
                Debug.Log("student does not have path");
                //Ensure there are at least two buildings (the student's current building, and a destination). Otherwise, will remove the student.
                if (gameManager.GetBuildingManager().GetBuildingCount() < 2)
                {
                    Debug.LogWarning("Insufficient buildings for students.");
                    student.Remove();
                    students.Remove(student);
                    continue;
                }
                else
                {
                    Debug.Log("Finding new destination for student");
                    //Find a random destination that the student isn't presently at.
                    Building destination;
                    do
                    {
                        destination = gameManager.GetBuildingManager().GetRandomBuilding();
                        Debug.Log("sm39");
                    } while (destination == null || student.GetPositionCell().GetBuilding() == null || destination.GetID() == student.GetPositionCell().GetBuilding().GetID());
                    student.SetDestination(destination);
                    //Calculate path to new destination
                    student.CalculatePath(grid);
                }
                
            }
            if (student.HasPath())
            {
                student.Update();
            }
        }
        Debug.Log("done updating");
    }
    /// <summary>
    /// Creates a new Student with a random position and destination
    /// </summary>
    /// <returns>The new Student's GameObject representation</returns>
    public GameObject CreateRandomStudent()
    {
        Debug.Log("sm55");
        if (gameManager.GetBuildingManager().GetBuildingCount() >= 2)
        {
            Student student = new Student(studentIndex++);
            //Determine random position/destination
            Cell position;
            Building destination;
            do
            {
                position = gameManager.GetBuildingManager().GetRandomBuilding().GetRandomEntrance();
                destination = gameManager.GetBuildingManager().GetRandomBuilding();
            } while (position.GetBuilding() == destination);
            //Set student attributes
            student.SetPosition(position);
            student.SetDestination(destination);
            student.CalculatePath(grid);
            students.Add(student);
            //Create gameObject for student
            GameObject gameObject = new GameObject("student" + student.GetID());
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            //TODO: Find a better way of doing this
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            meshFilter.mesh = primitive.GetComponent<MeshFilter>().mesh;
            Object.Destroy(primitive);
            meshRenderer.material = new Material(Shader.Find("Standard"));
            gameObject.transform.position = new Vector3(position.GetX(), 0, position.GetY());
            gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            student.SetGameObject(gameObject);
            Debug.Log("Created new student: ");
            return gameObject;
        }
        else
        {
            Debug.Log("Insufficient buildings to add new student");
            return null;
        }
    }

    public List<Student> GetListOfStudents()
    {
        return students;
    }
}
