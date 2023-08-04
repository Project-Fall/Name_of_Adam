using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SaveData
{
    public MapData MapData;
    public Incarna IncarnaData;
    public PlayerSkill UniversalPlayerSkillData;
    public List<DeckUnit> DeckUnitData;
    public int DarkEssence;
    public int PlayerSkillCount;
    public int DefaultMana;
    public int GuardCount;
}

public class SaveController
{
    string path;

    public SaveController()
    {
        path = Path.Combine(Application.persistentDataPath, "SaveData.json");
    }

    public void SaveGame()
    {
        SaveData newData = new SaveData();
        GameData CurGameData = GameManager.Data.GameData;

        newData.MapData = GameManager.Data.Map;

        newData.IncarnaData = CurGameData.Incarna;
        newData.DeckUnitData = CurGameData.DeckUnits;
        newData.UniversalPlayerSkillData = CurGameData.UniversalPlayerSkill;
        newData.DarkEssence = CurGameData.DarkEssence;
        newData.PlayerSkillCount = CurGameData.PlayerSkillCount;
        newData.DefaultMana = 50; // 
        newData.GuardCount = 1;   // 이 둘은 나중에 가져오기

        string json = JsonUtility.ToJson(newData, true);

        File.WriteAllText(path, json);
    }

    public void LoadGame()
    {
        string json =  File.ReadAllText(path);
        SaveData loadData = JsonUtility.FromJson<SaveData>(json);
        GameData CurGameData = GameManager.Data.GameData;

        GameManager.Data.Map = loadData.MapData;

        GameManager.Data.GameData.Incarna = loadData.IncarnaData;
        GameManager.Data.GameData.UniversalPlayerSkill = loadData.UniversalPlayerSkillData;
        GameManager.Data.GameData.DeckUnits = loadData.DeckUnitData;
        GameManager.Data.GameData.DarkEssence = loadData.DarkEssence;
        //CurGameData.DefaultMana
        //CurGameData.GuardCount
    }

    public bool SaveFileCheck() => File.Exists(Path.Combine(Application.persistentDataPath, "SaveData.json")) ? true : false;

    public void DeleteSaveData() => File.Delete(path);
}
