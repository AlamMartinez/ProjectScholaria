using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem
{
    private int totalScore;
    private int exp;
    private double overallHappiness;
    private int currLevel;
    private List<Student> students;
    private List<Building> buildings;
    private GameManager gameManager;
    private Grid grid;

    public ScoreSystem(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        overallHappiness = 0.0;
        exp = 0;
        totalScore = 0;
        currLevel = 0;
    }

    public double CalOverallHappiness()
    {
        int totalHappiness = 0;
        foreach (Student student in students)
        {
            totalHappiness += 80;//student.GetHappiness();
        }

        overallHappiness = (double)(totalHappiness / (students.Count));

        return overallHappiness;
    }

    public void SetStudentList(List<Student> studentList)
    {
        students = studentList;
    }

    public void SetBuildingList(List<Building> buildingList)
    {
        buildings = buildingList;
    }

    public void AddExp(int amount)
    {
        exp += amount;
    }

    public bool CheckLevelUp()
    {
        if (buildings.Count > (6 * (currLevel + 1)) && overallHappiness > 80.0) {
            currLevel++;
            return true;
        }
        return false;
    }

    public int GetLevel()
    {
        return currLevel;
    }

    public int CalcScore()
    {
        totalScore = (buildings.Count * 20) + (students.Count * (int)overallHappiness * 10) 
            + (currLevel * 100);
        return totalScore;
    }

    public int GetExp()
    {
        return exp;
    }
}
