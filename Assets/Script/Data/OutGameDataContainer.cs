using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

//NPC타락퀘스트 & 진척도 & 전당 유닛 등의 인자를 저장 & 불러오는 기능

[Serializable]
public class OutGameData
{
    public string Version;                   // 데이터 버전
    public int ProgressCoin;                 // 진척도 코인
    public List<ProgressSave> ProgressSaves; // 진척도 상점의 상품들
    public List<HallUnit> HallUnit;          // 전당 유닛
    
    public bool TutorialClear = false;
    public bool PhanuelClear = false;
    public bool SaviorClear = false;
    public bool YohrnClear = false;

    public bool IsVisitBaptism = false;
    public bool IsVisitStigmata = false;
    public bool IsVisitSacrifice = false;

    public int BaptismCorruptValue = 0;
    public int StigmataCorruptValue = 0;
    public int SacrificeCorruptValue = 0;

    public bool IsBaptismCorrupt = false;
    public bool IsStigmataCorrupt = false;
    public bool IsSacrificeCorrupt = false;

    public List<string> CutSceneViewData = new();
    public List<string> DialogViewData = new();

    public bool IsOnTooltipForDivineHallInBattle = false;
    public bool IsOnTooltipForDivineHall = false;
    public bool IsOnTooltipForSanctumInBattle = false;
    public bool IsOnTooltipForSanctum = false;

    public bool IsOnMainTooltipForYohrn = false;
    public bool IsOnMainTooltipForSavior = false;
    public bool IsOnMainTooltipForPhanuel = false;

    public float BattleSpeed = 1f;
    public int Language;
    public int Resolution;
    public bool IsWindowed;
    public float MasterSoundPower; // 0 ~ 1
    public float BGMSoundPower; // 0 ~ 1
    public float SESoundPower; // 0 ~ 1
    public bool IsReplayCutScene;
    public bool IsReplayDialog;

    //예전 데이터(세이브 파일 관리 용도로만 이용됨)
    public bool HorusClear = false; //SaviorClear로 수정됨
    public NPCQuest NpcQuest; //BaptismCorruptValue, StigmataCorruptValue, SacrificeCorruptValue로 수정됨
}

[Serializable]
public class HallUnit
{
    public string PrivateKey;     // 고유 Key
    public int ID;                // 전당 내에서 식별을 위한 ID
    public string UnitName;       // 지금은 유닛 이름으로 받고있지만 ID로 받는 기능이 추가되면 변경해야함
    public Stat UpgradedStat;     //업그레이드된 스텟
    public bool IsMainDeck;       //유닛이 메인덱에 포함되었는지 유무 확인
    public List<StigmaSaveData> Stigmata; // 유닛에게 추가된 성흔
    public List<UpgradeData> Upgrades; //유닛에게 추가된 강화

    public DeckUnit ConvertToDeckUnit()
    {
        DeckUnit deckUnit = new DeckUnit();

        deckUnit.PrivateKey = PrivateKey;
        deckUnit.HallUnitID = ID;
        deckUnit.IsMainDeck = IsMainDeck;
        deckUnit.Data = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/{UnitName}");
        deckUnit.DeckUnitUpgradeStat = UpgradedStat;
        deckUnit.SetStigmaSaveData(Stigmata);
        deckUnit.SetUpgrade(Upgrades);

        return deckUnit;
    }
}

[Serializable]
public class ProgressItem
{
    public int ID;             // 진척도 ID
    public string Name;        // 이름
    public int Cost;           // 가격
    public int Prequest;       // 선행 조건 ID
    public string Description; // 설명
}

[Serializable]
public class ProgressSave
{
    public int ID;
    public bool isUnLock;

    public ProgressSave(int id, bool isUnLock)
    {
        this.ID = id;
        this.isUnLock = isUnLock;
    }
}

public class OutGameDataContainer : MonoBehaviour
{
    private const string encryptionKey = "EncryptOutGameData!@#$%^&*()_+";

    //데모용
    private const string OutGameDataFileName = "126634399755.dat";

    //정식용
    //private const string OutGameDataFileName = "126634399756.dat";
    //private const string OutGameDataFileNameReadable = "readable.dat";

    private SaveVersionController _versionController;
    public Dictionary<int, ProgressItem> ProgressItems = new();

    private string _path;
    private List<Resolution> _resolutions;

    public OutGameData Data;

    public void Init()
    {
        _versionController = new SaveVersionController();
        ProgressItems = LoadJson<ProgressLoader, int, ProgressItem>("ProgressData").MakeDict();

        // 사용자\AppData\localLow에 있는 SaveData.json의 경로
        _path = Path.Combine(Application.persistentDataPath, OutGameDataFileName);

        _resolutions = new() {
            GetResolution(1920, 1080, 144),
            GetResolution(1280, 720, 144),
            GetResolution(720, 480, 144)
        };

        LoadData();
        SetResolution();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    private string EncryptAndDecrypt(string data)
    {
        string result = string.Empty;
        for (int i = 0; i < data.Length; i++)
            result += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        return result;
    }

    public void SaveData()
    {
        DataIntegrityCheck();

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(_path, EncryptAndDecrypt(json));
        //File.WriteAllText(Path.Combine(Application.persistentDataPath, OutGameDataFileNameReadable), json);
    }

    public void DataIntegrityCheck()
    {
        List<HallUnit> mainDeckHallUnits = new();

        foreach (HallUnit hallUnit in Data.HallUnit)
        {
            if (hallUnit.IsMainDeck)
            {
                mainDeckHallUnits.Add(hallUnit);
            }
        }

        if (mainDeckHallUnits.Count > 4)
        {
            Debug.LogError("Main Deck Over 4 Error");
            for (int i = 4; i < mainDeckHallUnits.Count; i++)
            {
                Debug.LogError("Error Unit:" + mainDeckHallUnits[i].PrivateKey);
                Debug.LogError("Error Unit:" + mainDeckHallUnits[i].UnitName);
                Debug.LogError("Error Unit:" + mainDeckHallUnits[i].ID);
                mainDeckHallUnits[i].IsMainDeck = false;
            }
        }
    }

    public void LoadData()
    {
        try
        {
            // AppData에 파일이 있으면 OutGameData파일이 있으면 불러오기
            string json = File.ReadAllText(_path);
            Data = JsonUtility.FromJson<OutGameData>(EncryptAndDecrypt(json));

            if (_versionController.CheckNeedMigration() == true)
            {
                // 데이터 무결성 검사
                _versionController.MigrateData();
            }

            DataIntegrityCheck();
        }
        catch(FileNotFoundException e)
        {
            // 파일이 없을 경우(처음 플레이 시 or 데이터 삭제 시) 새로운 데이터 생성
            CreateData();
        }
    }

    public List<DeckUnit> SetHallDeck()
    {
        List<DeckUnit> HallList = new();

        foreach (HallUnit hallUnit in Data.HallUnit)
        {
            DeckUnit deckUnit = hallUnit.ConvertToDeckUnit();
            HallList.Add(deckUnit);
        }

        return HallList;
    }

    public void CreateData()
    {
        // Resources폴더 안에 있는 데이터를 복사하여 저장
        TextAsset text = GameManager.Resource.Load<TextAsset>("Data/OutGameData");
        Data = JsonUtility.FromJson<OutGameData>(text.text);
        Data.Version = Application.version;

        SetProgressInit();
        ResetOption();
        SaveData();
    }

    public ProgressItem GetProgressItem(int ID) => ProgressItems[ID];

    public ProgressSave GetProgressSave(int ID) => Data.ProgressSaves.Find(x => x.ID == ID);

    public bool IsUnlockedItem(int ID) => GetProgressSave(ID).isUnLock;
    public bool IsUnlockedItem(SanctumUnlock ID) => GetProgressSave(ID.GetHashCode()).isUnLock;

    // 현재 확인하는 아이템이 구매 가능한 아이템인지 확인
    public bool GetBuyable(int ID)
    {
        ProgressItem item = GetProgressItem(ID);

        if (!IsUnlockedItem(item.Prequest) || IsUnlockedItem(ID))
        {
            return false;
        }

        return true;
    }

    public void BuyProgressItem(int ID)
    {
        ProgressItem item = GetProgressItem(ID);
        GetProgressSave(ID).isUnLock = true;
        Data.ProgressCoin -= item.Cost;
        SaveData();
    }

    public HallUnit FindHallUnitID(int ID) => Data.HallUnit.Find(x => x.ID == ID);

    public List<HallUnit> FindHallUnitList() => Data.HallUnit;

    public void AddHallUnit(DeckUnit unit)
    {
        Debug.Log($"{unit.Data.Name} Unit Get");
        unit.DeckUnitUpgradeStat.FallCurrentCount = 0;

        HallUnit newUnit = new()
        {
            ID = GameManager.Data.GetHallUnitID(),
            PrivateKey = unit.PrivateKey,
            UnitName = unit.Data.ID,
            UpgradedStat = unit.DeckUnitUpgradeStat,
            IsMainDeck = false,
            Stigmata = unit.GetStigmaSaveData(),
            Upgrades = unit.GetUpgradeData()
        };

        Data.HallUnit.Add(newUnit);
        SaveData();
    }

    public void CoverHallUnit(DeckUnit unit)
    {
        HallUnit coverUnit = null;

        foreach (HallUnit hallUnit in Data.HallUnit)
        {
            if (hallUnit.PrivateKey == unit.PrivateKey)
            {
                coverUnit = hallUnit;
                break;
            }
        }

        if (coverUnit == null)
        {
            Debug.LogError("전당에 등록되지 않은 유닛입니다.");
            return;
        }

        Debug.Log($"{unit.Data.Name} Unit Get");

        unit.DeckUnitUpgradeStat.FallCurrentCount = 0;

        coverUnit.UnitName = unit.Data.ID;
        coverUnit.IsMainDeck = true;
        coverUnit.ID = unit.HallUnitID;
        coverUnit.Stigmata = unit.GetStigmaSaveData();
        coverUnit.Upgrades = unit.GetUpgradeData();
        coverUnit.UpgradedStat = unit.DeckUnitUpgradeStat;

        SaveData();

        GameManager.Data.GameData.FallenUnits.Clear();
        SceneChanger.SceneChange("MainScene");
    }

    public void ResetNPCQuest()
    {
        Data.IsVisitBaptism = false;
        Data.IsVisitStigmata = false;
        Data.IsVisitSacrifice = false;

        Data.BaptismCorruptValue = 0;
        Data.StigmataCorruptValue = 0;
        Data.SacrificeCorruptValue = 0;
        SaveData();
    }

    public void RemoveHallUnit(int ID)
    {
        Data.HallUnit.Remove(FindHallUnitID(ID));
    }

    public void RemoveHallUnit(string privateKey)
    {
        Data.HallUnit.Remove(Data.HallUnit.Find(x => x.PrivateKey == privateKey));
    }

    public void DeleteAllData() => File.Delete(_path);

    private Resolution GetResolution(int width, int height, int refreshRate)
    {
        Resolution resolution = new();
        resolution.width = width;
        resolution.height = height;
        resolution.refreshRate = refreshRate;
        return resolution;
    }

    public void SetResolution()
    {
        Resolution resolution = _resolutions[Data.Resolution];
        Screen.SetResolution(resolution.width, resolution.height, !Data.IsWindowed);
        SaveData();
    }

    public void SetProgressInit()
    {
        Data.ProgressSaves = new List<ProgressSave>();
        foreach (var item in ProgressItems)
            Data.ProgressSaves.Add(new ProgressSave(item.Key, false));

        GetProgressSave(0).isUnLock = true;
        GetProgressSave(50).isUnLock = true;
        GetProgressSave(51).isUnLock = true;
        GetProgressSave(60).isUnLock = true;
        GetProgressSave(70).isUnLock = true;
    }

    public void ResetOption()
    {
        Data.Resolution = 0;
        Data.IsWindowed = false;
        Data.MasterSoundPower = Data.BGMSoundPower = Data.SESoundPower = 0.5f;
        Data.BattleSpeed = 1f;
        Data.Language = 0;
        //Data.Language = GameManager.Steam.GetCurrentGameLanguage();
        Data.IsReplayCutScene = false;
        Data.IsReplayDialog = false; ;
    }

    public List<Resolution> GetAllResolution() => _resolutions;

    public Resolution GetResolution() => _resolutions[Data.Resolution];

    public void SetCutSceneData(CutSceneType cutSceneType, bool isDone)
    {
        string cutSceneKey = cutSceneType.ToString();

        if (Data.CutSceneViewData.Contains(cutSceneKey) && !isDone)
        {
            Data.CutSceneViewData.Remove(cutSceneKey);
        }
        else if (!Data.CutSceneViewData.Contains(cutSceneKey) && isDone)
        {
            Data.CutSceneViewData.Add(cutSceneKey);
        }

        SaveData();
    }

    public bool CutScenePlayCheck(CutSceneType cutSceneType) => !Data.CutSceneViewData.Contains(cutSceneType.ToString()) || Data.IsReplayCutScene;

    public void SetDialogData(string unitName, bool isDone)
    {
        if (Data.DialogViewData.Contains(unitName) && !isDone)
        {
            Data.DialogViewData.Remove(unitName);
        }
        else if (!Data.DialogViewData.Contains(unitName) && isDone)
        {
            Data.DialogViewData.Add(unitName);
        }
        SaveData();
    }

    public bool DialogPlayCheck(string unitName) => !Data.DialogViewData.Contains(unitName) || Data.IsReplayDialog;
}