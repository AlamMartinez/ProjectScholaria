using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SaveData _saveData;
    private GameManager _gameMgr;

    public SaveManager(GameManager gm)
    {
        this._gameMgr = gm;
        this._saveData = new SaveData();
        this._saveData.buildings = new BuildingSaves(gm);
        this._saveData.paths = new PathSaves(gm);
        this._saveData.students = new StudentSaves();
    }

    [System.Serializable]
    public struct SaveData
    {
        public BuildingSaves buildings;
        public PathSaves paths;
        public StudentSaves students;
    }

    public static string GetSaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".dat";
        return saveFile;
    }

    public void SerializeToSaveFile()
    {
        Debug.Log("Copying game state");
        _saveData.buildings.Save();
        _saveData.paths.Save();
        _saveData.students.amount = _gameMgr.GetGameState().numStudents;

        Debug.Log("Writing to save file");
        File.WriteAllText(GetSaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    public void LoadFromSaveFile()
    {
        string saveFileName = GetSaveFileName();
        Debug.Log("Atempting to load from save file: " + saveFileName);
        string saveContent = File.ReadAllText(saveFileName);
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        //if(_saveData == null)
        //{
        //    Debug.LogError("Something wrong with reading from save file");
        //}

        _saveData.buildings.buildingManager = _gameMgr.GetBuildingManager();
        _saveData.paths.gm = _gameMgr;
        _saveData.buildings.Load();
        _saveData.paths.Load();
        for(int i = 0; i < _saveData.students.amount; i++)
        {
            _gameMgr.AddRandomStudent();
        }
    }
}
[System.Serializable]
public class PathSaves
{
    public List<PathSave> pathSaves;
    public GameManager gm;
    public PathSaves(GameManager gm) {
        this.gm = gm;
        this.pathSaves = new List<PathSave>();
    }

    public void Save()
    {
        foreach(GameObject path in gm.GetGameObjects())
        {
            if(path.tag == "Path")
            {
                Debug.Log("Saved path");
                PathSave pathSave = new PathSave();
                pathSave.pos = new Vector2Int((int)path.transform.position.x, (int)path.transform.position.z);
                pathSaves.Add(pathSave);
            }
        }
    }

    public void Load()
    {
        foreach (PathSave pathSave in pathSaves)
        {
            gm.AddPath(pathSave.pos.x, pathSave.pos.y);
            Debug.Log("Loaded path");
        }
    }
}

[System.Serializable]
public class BuildingSaves
{
    public List<BuildingSave> bdSaves;
    public BuildingManager buildingManager;

    public BuildingSaves(GameManager gm) {
        this.buildingManager = gm.GetBuildingManager();
        this.bdSaves = new List<BuildingSave>();
    }

    public void Save()
    {
        foreach(Building bd in buildingManager.GetBuildings())
        {
            Debug.Log("Saved building");
            BuildingSave bdSave = new BuildingSave();
            bdSave.id = bd.GetID();
            bdSave.type = bd.GetBuildingType();
            bdSave.name = bd.GetName();
            bdSave.visits = bd.GetVisits();
            bdSave.boardPos = bd.GetBoardPos();
            bdSave.template = bd.GetTemplate();
            bdSaves.Add(bdSave);
        }

    }

    public void Load() {
        foreach(BuildingSave bs in bdSaves)
        {
            buildingManager.ConstructBuildingFromSave(bs);
            Debug.Log("Loaded building");
        }
    }
}

[System.Serializable]
public struct PathSave
{
    public Vector2Int pos;
}

[System.Serializable]
public struct BuildingSave
{
    public int id;
    public int type;
    public string name;
    public int visits;
    public int template;
    public Vector2Int boardPos;
}

[System.Serializable]
public struct StudentSaves
{
    public int amount;
}
