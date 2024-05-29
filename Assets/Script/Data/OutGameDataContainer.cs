using System;
using System.IO;
using System.Collections;
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
    public bool HorusClear = false;

    public bool IsVisitUpgrade = false;
    public bool IsVisitStigma = false;
    public bool IsVisitDarkShop = false;
    public NPCQuest NpcQuest;                //npc타락퀘스트
    public bool IsGameOver = false;          //바로 전에 게임이 오버되었는지 체크
    public bool[] cutSceneData = new bool[Enum.GetValues(typeof(CutSceneType)).Length];        //현재 진행된 컷씬
    public bool IsOnMainTooltipForHorus = false;
    public bool IsOnMainTooltipForPhanuel = false;

    public int language;
    public int resolution;
    public bool IsWindowed;
    public float MasterSoundPower; // 0 ~ 1
    public float BGMSoundPower; // 0 ~ 1
    public float SESoundPower; // 0 ~ 1
}

[Serializable]
public class HallUnit
{
    public string PrivateKey;     // 고유 Key
    public int ID;                // 전당 내에서 식별을 위한 ID
    public string UnitName;       // 지금은 유닛 이름으로 받고있지만 ID로 받는 기능이 추가되면 변경해야함
    public Stat UpgradedStat;     //업그레이드된 스텟
    public bool IsMainDeck;       //유닛이 메인덱에 포함되었는지 유무 확인
    public List<StigmaSaveData> Stigmata; // 유닛에게 추가된 낙인
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
    //private const string OutGameDataFileName = "126634399755.dat";

    //정식용
    private const string OutGameDataFileName = "126634399756.dat";

    private SaveVersionController _versionController;

    // 현재 진행중인 게임에서 관리하는 아웃게임데이터
    private Dictionary<int, ProgressItem> ProgressItems = new();
    private OutGameData _data;
    private string _path;
    private List<Resolution> _resolutions;

    public void Init()
    {
        ProgressItems = LoadJson<ProgressLoader, int, ProgressItem>("ProgressData").MakeDict();

        _versionController = new SaveVersionController();

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
        string json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(_path, EncryptAndDecrypt(json));
    }

    public void LoadData()
    {
        try
        {
            // AppData에 파일이 있으면 OutGameData파일이 있으면 불러오기
            string json = File.ReadAllText(_path);
            _data = JsonUtility.FromJson<OutGameData>(EncryptAndDecrypt(json));

            if (_versionController.CheckNeedMigration() == true)
            {
                // 데이터 무결성 검사
                _versionController.MigrateData();
            }
        }
        catch(FileNotFoundException e)
        {
            // 파일이 없을 경우(처음 플레이 시 or 데이터 삭제 시) 새로운 데이터 생성
            Debug.Log(e);
            CreateData();
        }
    }

    public List<DeckUnit> SetHallDeck()
    {
        List<DeckUnit> HallList = new();

        foreach (HallUnit hallUnit in _data.HallUnit)
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
        _data = JsonUtility.FromJson<OutGameData>(text.text);
        _data.Version = Application.version;

        SetLanguage(2);
        GameManager.Locale.LanguageChanged(GetLanguage());

        SetProgressInit();
        ReSetOption();
        SaveData();
    }

    public void SetProgressCoin(int num)
    {
        _data.ProgressCoin += num;
        SaveData();
    }

    public int GetProgressCoin() => _data.ProgressCoin;

    public ProgressItem GetProgressItem(int ID) => ProgressItems[ID];

    public ProgressSave GetProgressSave(int ID) => _data.ProgressSaves.Find(x => x.ID == ID);

    public bool IsUnlockedItem(int ID) => GetProgressSave(ID).isUnLock;

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
        SetProgressCoin(-item.Cost);
    }

    public HallUnit FindHallUnitID(int ID) => _data.HallUnit.Find(x => x.ID == ID);

    public List<HallUnit> FindHallUnitList() => _data.HallUnit;

    public void AddHallUnit(DeckUnit unit)
    {
        Debug.Log($"{unit.Data.Name} Unit Get");
        unit.DeckUnitUpgradeStat.FallCurrentCount = 0;

        HallUnit newUnit = new();

        newUnit.ID = GameManager.Data.GetHallUnitID();
        newUnit.PrivateKey = unit.PrivateKey;
        newUnit.UnitName = unit.Data.ID;
        newUnit.UpgradedStat = unit.DeckUnitUpgradeStat;
        newUnit.IsMainDeck = false;
        newUnit.Stigmata = unit.GetStigmaSaveData();
        newUnit.Upgrades = unit.GetUpgradeData();

        _data.HallUnit.Add(newUnit);
        SaveData();

        GameManager.Data.GameData.FallenUnits.Clear();
        SceneChanger.SceneChange("MainScene");
    }

    public void CoverHallUnit(DeckUnit unit)
    {
        HallUnit coverUnit = null;

        foreach (HallUnit hallUnit in _data.HallUnit)
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

    public void DoneTutorial(bool isclear)
    {
        _data.TutorialClear = isclear;
        SaveData();
    }

    public void ClearPhanuel(bool isclear)
    {
        _data.PhanuelClear = isclear;
        SaveData();
    }

    public void ClearHorus(bool isclear)
    {
        _data.HorusClear = isclear;
        SaveData();
    }

    public bool IsPhanuelClear() => _data.PhanuelClear;

    public bool IsHorusClear() => _data.HorusClear;

    public bool IsTutorialClear() => _data.TutorialClear;

    public NPCQuest GetNPCQuest() => _data.NpcQuest;

    public bool GetVisitUpgrade() => _data.IsVisitUpgrade;

    public void SetVisitUpgrade(bool isVisit)
    {
        _data.IsVisitUpgrade = isVisit;
        SaveData();
    }
    public bool GetVisitStigma() => _data.IsVisitStigma;

    public void SetVisitStigma(bool isVisit)
    {
        _data.IsVisitStigma = isVisit;
        SaveData();
    }
    public bool GetVisitDarkshop() => _data.IsVisitDarkShop;

    public void SetVisitDarkshop(bool isVisit)
    {
        _data.IsVisitDarkShop = isVisit;
        SaveData();
    }

    public bool IsGameOverCheck() => _data.IsGameOver;

    public void SetIsGameOverCheck(bool isGameOver) {
        _data.IsGameOver = isGameOver;
        SaveData();
    }

    public void ResetNPCQuest()
    {
        Debug.Log("npc데이터 리셋되었다.");
        _data.IsVisitUpgrade = false;
        _data.IsVisitDarkShop = false;
        _data.IsVisitStigma = false;
        _data.NpcQuest.ClearQuest();
        SaveData();
    }
    public void RemoveHallUnit(int ID)
    {
        _data.HallUnit.Remove(FindHallUnitID(ID));
    }

    public void RemoveHallUnit(string privateKey)
    {
        _data.HallUnit.Remove(_data.HallUnit.Find(x => x.PrivateKey == privateKey));
    }

    public void DeleteAllData() => File.Delete(_path);

    private Resolution GetResolution(int width, int height, int refreshRate)
    {
        Resolution resolution = new Resolution();
        resolution.width = width;
        resolution.height = height;
        resolution.refreshRate = refreshRate;
        return resolution;
    }

    private void SetResolution()
    {
        Resolution resolution = _resolutions[_data.resolution];
        Screen.SetResolution(resolution.width, resolution.height, !_data.IsWindowed);
        SaveData();
    }

    private void SetProgressInit()
    {
        _data.ProgressSaves = new List<ProgressSave>();
        foreach (var item in ProgressItems)
            _data.ProgressSaves.Add(new ProgressSave(item.Key, false));
        GetProgressSave(0).isUnLock = true;
        GetProgressSave(50).isUnLock = true;
        GetProgressSave(51).isUnLock = true;
        GetProgressSave(60).isUnLock = true;
        GetProgressSave(70).isUnLock = true;
    }

    public void SetWindow(bool isWindowed)
    {
        _data.IsWindowed = isWindowed;
        SetResolution();
    }

    public void SetResolution(int resolution)
    {
        _data.resolution = resolution;
        SetResolution();
    }

    public List<Resolution> GetAllResolution() => _resolutions;
    public int GetLanguage() => _data.language;
    public int SetLanguage(int language) => _data.language = language;
    public int GetResolutionIndex() => _data.resolution;
    public Resolution GetResolution() => _resolutions[_data.resolution];
    public bool IsWindowed() => _data.IsWindowed;
    public float GetMasterSoundPower() => _data.MasterSoundPower;
    public float GetBGMSoundPower() => _data.BGMSoundPower;
    public float GetSESoundPower() => _data.SESoundPower;
    public float SetMasterSoundPower(float power) => _data.MasterSoundPower = power;
    public float SetBGMSoundPower(float power) => _data.BGMSoundPower = power;
    public float SetSESoundPower(float power) => _data.SESoundPower = power;
    public void ReSetOption()
    {
        _data.resolution = 0;
        _data.IsWindowed = false;
        _data.MasterSoundPower = _data.BGMSoundPower = _data.SESoundPower = 0.5f;
    }
    public bool GetCutSceneData(CutSceneType cutSceneType) => _data.cutSceneData[(int)cutSceneType];
    public void SetCutSceneData(CutSceneType cutSceneType, bool isDone) => _data.cutSceneData[(int)cutSceneType] = isDone;
    public bool GetIsOnMainTooltipForHorus() => _data.IsOnMainTooltipForHorus;
    public bool GetIsOnMainTooltipForPhanuel() => _data.IsOnMainTooltipForPhanuel;
    public void SetIsOnMainTooltipForHorus(bool isOn) => _data.IsOnMainTooltipForHorus = isOn;
    public void SetIsOnMainTooltipForPhanuel(bool isOn) => _data.IsOnMainTooltipForPhanuel = isOn;
    public string GetVersion() => _data.Version;
    public void SetVersion(string version) => _data.Version = version;
}