using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public MapDataSaveData MapData;
    public string IncarnaID;
    public List<SaveUnit> DeckUnitData;
    public List<SaveUnit> FallenUnitsData;
    public int DarkEssence;
    public int PlayerHP;
    public Progress ProgressData;
    public int StageAct;
    public Vector3 StageDivine;
    public int CurrentAct;
}

[Serializable]
public class SaveUnit
{
    public string PrivateKey;
    public string UnitDataID;
    public int HallID;
    public bool IsMainDeck;
    public Stat UnitStat;
    public List<StigmaSaveData> Stigmata; // 유닛에게 추가된 낙인
    public List<UpgradeData> Upgrades; // 유닛에게 추가된 강화

    public DeckUnit ConventToDeckUnit()
    {
        DeckUnit deckUnit = new();

        deckUnit.PrivateKey = PrivateKey;
        deckUnit.Data = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/{UnitDataID}");
        deckUnit.DeckUnitUpgradeStat = UnitStat;
        deckUnit.SetStigmaSaveData(Stigmata);
        deckUnit.SetUpgrade(Upgrades);
        deckUnit.HallUnitID = HallID;
        deckUnit.IsMainDeck = IsMainDeck;

        return deckUnit;
    }
}

public class SaveController : MonoBehaviour
{
    private const string encryptionKey = "EncryptSaveData!@#$%^&*()_+";

    //데모용
    private const string SaveDataFileName = "867755930112.dat";

    //출시용
    //private const string SaveDataFileName = "867755930113.dat";

    string path;

    public void Init()
    {
        // 사용자/Appdata//LocalLow에 자신의 파일이 있는지 확인하는 함수(System.IO 필요)
        path = Path.Combine(Application.persistentDataPath, SaveDataFileName);
    }

    // 게임 진행 정보 저장
    // 저장하는 데이터가 늘어나면 이곳에서 관리해주어야 함
    public void SaveGame()
    {
        List<SaveUnit> saveDeckUnitList = new();
        List<SaveUnit> saveFallenUnitList = new();
        SaveData newData = new();
        GameData curGameData = GameManager.Data.GameData;

        foreach (DeckUnit unit in curGameData.DeckUnits)
        {
            SaveUnit saveUnit = unit.ConventToSaveUnit();
            saveDeckUnitList.Add(saveUnit);
        }

        foreach (DeckUnit unit in curGameData.FallenUnits)
        {
            SaveUnit saveUnit = unit.ConventToSaveUnit();
            saveFallenUnitList.Add(saveUnit);
        }

        newData.MapData = GameManager.Data.GetMapSaveData();
        newData.DeckUnitData = saveDeckUnitList;
        newData.FallenUnitsData = saveFallenUnitList;
        newData.IncarnaID = curGameData.Incarna.ID;
        newData.DarkEssence = curGameData.DarkEssence;
        newData.PlayerHP = curGameData.PlayerHP;
        newData.ProgressData = curGameData.Progress;
        newData.StageAct = GameManager.Data.StageAct;
        newData.StageDivine = curGameData.StageDivine;
        newData.CurrentAct = curGameData.CurrentAct;

        string json = JsonUtility.ToJson(newData, true);

        File.WriteAllText(path, EncryptAndDecrypt(json));
    }

    public SaveData GetSaveData()
    {
        string json = File.ReadAllText(path);
        SaveData loadData = JsonUtility.FromJson<SaveData>(EncryptAndDecrypt(json));
        return loadData;
    }

    public void SaveData(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, EncryptAndDecrypt(json));
    }

    // 게임 진행 정보 저장
    // 저장하는 데이터가 늘어나면 이곳에서 관리해주어야 함
    public void LoadGame()
    {
        string json = File.ReadAllText(path);
        SaveData loadData = JsonUtility.FromJson<SaveData>(EncryptAndDecrypt(json));

        GameManager.Data.SetMapSaveData(loadData.MapData);
        List<DeckUnit> savedDeckUnitList = new();
        List<DeckUnit> savedFallenUnitList = new();

        foreach (SaveUnit saveUnit in loadData.DeckUnitData)
        {
            DeckUnit deckunit = saveUnit.ConventToDeckUnit();
            savedDeckUnitList.Add(deckunit);
        }

        foreach (SaveUnit saveUnit in loadData.FallenUnitsData)
        {
            DeckUnit deckunit = saveUnit.ConventToDeckUnit();
            savedFallenUnitList.Add(deckunit);
        }

        // FallenUnits와 DeckUnits에 겹치는 유닛이 있기 때문에 동일한 유닛을 참조하도록 함
        foreach (DeckUnit deckUnit in savedDeckUnitList)
        {
            for (int i = 0; i < savedFallenUnitList.Count; i++)
            {
                DeckUnit fallenUnit = savedFallenUnitList[i];
                if (deckUnit.IsEqual(fallenUnit))
                {
                    savedFallenUnitList[i] = deckUnit;
                    //break;
                }
            }
        }

        GameManager.Data.GameData.DeckUnits = savedDeckUnitList;
        GameManager.Data.GameData.FallenUnits = savedFallenUnitList;
        GameManager.Data.GameData.Incarna = GameManager.Resource.Load<Incarna>($"ScriptableObject/Incarna/{loadData.IncarnaID}"); ;
        GameManager.Data.GameData.DarkEssence = loadData.DarkEssence;
        GameManager.Data.GameData.PlayerHP = loadData.PlayerHP;
        GameManager.Data.GameData.Progress = loadData.ProgressData;
        GameManager.Data.StageAct = loadData.StageAct;
        GameManager.Data.GameData.StageDivine = loadData.StageDivine;
        GameManager.Data.GameData.CurrentAct = loadData.CurrentAct;
    }

    // 저장된 데이터가 있는지 확인
    public bool SaveFileCheck() => File.Exists(Path.Combine(Application.persistentDataPath, SaveDataFileName));

    // 저장된 데이터 삭제
    public void DeleteSaveData() => File.Delete(path);

    private string EncryptAndDecrypt(string data)
    {
        string result = string.Empty;
        for (int i = 0; i < data.Length; i++)
            result += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        return result;
    }
}