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
        //Create gameObject for student
        GameObject gameObject = new GameObject("student" + student.GetID());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //TODO: Find a better way of doing this
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        meshFilter.mesh = primitive.GetComponent<MeshFilter>().mesh;
        Object.Destroy(primitive);
        meshRenderer.material = new Material(Shader.Find("Standard"));
        gameObject.transform.position = new Vector3(student.GetPositionCell().GetX(), 0, student.GetPositionCell().GetY());
        gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        student.SetGameObject(gameObject);
        Debug.Log("Created new student: ");
        return gameObject;
    }

    public List<Student> GetListOfStudents()
    {
        return students;
    }

    public void DecreaseHappiness()
    {
        foreach (Student student in students)
        {
            student.DecreaseHappiness(5);
        }
        return;
    }
}
