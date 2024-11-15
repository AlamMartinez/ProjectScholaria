using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SaveData _saveData;

    public SaveManager(GameManager gm)
    {
        this._saveData = new SaveData();
        this._saveData.buildings = new BuildingSaves(gm);
    }

    [System.Serializable]
    public struct SaveData
    {
        public BuildingSaves buildings;
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

        Debug.Log("Writing to save file");
        File.WriteAllText(GetSaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    public static void LoadFromSaveFile(GameManager gm)
    {
        string saveFileName = GetSaveFileName();
        Debug.Log("Atempting to load from save file: " + saveFileName);
        string saveContent = File.ReadAllText(saveFileName);
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        //if(_saveData == null)
        //{
        //    Debug.LogError("Something wrong with reading from save file");
        //}

        _saveData.buildings.buildingManager = gm.GetBuildingManager();
        _saveData.buildings.Load();
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
            bdSave.type = bd.GetTypeID();
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
public struct BuildingSave
{
    public int id;
    public int type;
    public string name;
    public int visits;
    public int template;
    public Vector2Int boardPos;
}
