using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private SaveData _saveData;
    private GameManager gameManager;

    public SaveManager(GameManager gm)
    {
        gameManager = gm;
        _saveData = new SaveData();
    }

    [System.Serializable]
    public struct SaveData
    {
        public BuildingSaves bdSaves;
    }

    public static string GetSaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".dat";
        return saveFile;
    }

    public void SerializeToSaveFile(BuildingManager bm)
    {
        CopyToSave(bm);

        File.WriteAllText(GetSaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private void CopyToSave(BuildingManager bm)
    {
        _saveData.bdSaves.Save(bm);
    }
}

[System.Serializable]
public class BuildingSaves
{
    public List<BuildingSave> bdSaves;

    public void Save(BuildingManager bm)
    {
        Debug.Log("Saved building");
        foreach(Building bd in bm.GetBuildings())
        {
            BuildingSave bdSave = new BuildingSave();
            bdSave.id = bd.GetID();
            bdSave.type = bd.GetType();
            bdSave.name = bd.GetName();
            bdSave.visits = bd.GetVisits();
            bdSaves.Add(bdSave);
        }

    }

    void Load(ref GameManager gameManager) {

    }
}

[System.Serializable]
public struct BuildingSave
{
    public int id;
    public string type;
    public string name;
    public int visits;
}
