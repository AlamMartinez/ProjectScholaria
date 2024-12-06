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
    /// Updates the position, pathfinding, and schedule of every student.
    /// </summary>
    public void Update()
    {
        foreach (Student student in students)
        {
            if(!student.HasSchedule())
            {
                student.GenerateSchedule(gameManager.GetBuildingManager());
            }
            student.Update(grid);
        }
    }
    /// <summary>
    /// Creates a new Student with a random schedule
    /// </summary>
    /// <returns>The new Student's GameObject representation</returns>
    public GameObject CreateStudent()
    {
        Student student = new Student(studentIndex++);
        //Generate schedule for student
        student.GenerateSchedule(gameManager.GetBuildingManager());
        students.Add(student);
        GameObject template = Resources.Load<GameObject>("Misc/student");
        //Create gameObject for student
        GameObject gameObject = new GameObject("student" + student.GetID());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //TODO: Find a better way of doing this
        meshFilter.mesh = template.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.material = Resources.Load<Material>("Misc/student");
        gameObject.transform.position = new Vector3(student.GetPositionCell().GetX(), 0.5f, student.GetPositionCell().GetY());
        gameObject.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        student.SetGameObject(gameObject);
        Debug.Log("Created new student: ");
        return gameObject;
    }

    public List<Student> GetListOfStudents()
    {
        return students;
    }
}
