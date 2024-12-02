using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem
{
    private int totalScore;
    private int exp;
    private int expCurrCap;
    private double overallHappiness;
    private int currLevel;
    private List<Student> students;
    private List<Building> buildings;
    private GameManager gameManager;
    private PlacementManager placementManager;

    public ScoreSystem(GameManager gameManager, PlacementManager placementManager)
    {
        this.gameManager = gameManager;
        this.placementManager = placementManager;
        overallHappiness = 0.0;
        exp = 0;
        expCurrCap = 50;
        totalScore = 0;
        currLevel = 0;
        students = null;
        buildings = null;
    }

    public double CalOverallHappiness()
    {
        if (students.Count == 0) { return 0.0; }

        double totalHappiness = 0;
        foreach (Student student in students)
        {
            totalHappiness += student.GetHappiness();
        }

        overallHappiness = (double)totalHappiness / (double)students.Count;

        return overallHappiness;
    }

    public void SetStudentList(List<Student> studentList)
    {
        if (studentList == null) { return; }
        students = studentList;
    }

    public void SetBuildingList(List<Building> buildingList)
    {
        if (buildingList == null) { return; }
        buildings = buildingList;
    }

    public void UpdateExp()
    {
        int currExp = 0;
        //Debug.Log("exp of system is... " + exp);
        if (students.Count == 0 || buildings.Count == 0)
        {
            exp = 0;
            return;
        }

        //Debug.Log("students size is... " + students.Count);

        currExp += buildings.Count * 5;
        currExp = currExp + (int)((students.Count * 10.0) * (overallHappiness / 100.0));
        currExp += placementManager.GetBusStopCount() * 2;

        exp = currExp;
    }

    public bool CheckLevelUp()
    {
        if (exp >= expCurrCap)
        {
            expCurrCap = ((currLevel + 1) * 50) + 50;
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
        if (students.Count == 0 || buildings.Count == 0)
        {
            return 0;
        }
        totalScore = (buildings.Count * 20) + (students.Count * (int)overallHappiness * 10)
            + (currLevel * 100);
        return totalScore;
    }

    public int GetExp()
    {
        return exp;
    }
}