using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : MonoBehaviour
{
    // public Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, List<StageSpawnData>> StageDatas = new();
    public MapData Map;
    public int StageAct; // 현재 맵이 몇 막인지 기록하는 변수. 0 : 튜토리얼, 1 : 1막, 2 : 2막 이런식으로

    private CutSceneType _cutSceneToDisplay; // 출력할 컷씬 기록하는 변수
    public CutSceneType CutSceneToDisplay
    {
        get => _cutSceneToDisplay;
        set
        {
            Debug.Log($"컷씬 {_cutSceneToDisplay} -> {value} 변경");
            _cutSceneToDisplay = value;
        }
    }

    [SerializeField] public GameData GameData;
    [SerializeField] public GameData GameDataMain;
    [SerializeField] public GameData GameDataTutorial;

    [SerializeField] public GameData GameDataMainLayout; //디버깅용 데이터

    public Dictionary<string, List<Script>> ScriptData = new();
    public StigmaController StigmaController;
    public UpgradeController UpgradeController;

    public void Init()  
    {
        // StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
        StageDatas = LoadJson<StageLoader, int, List<StageSpawnData>>("StageData").MakeDict();
        ScriptData = LoadJson<ScriptLoader, string, List<Script>>("Script").MakeDict();

        StigmaController = new StigmaController();
        UpgradeController = new UpgradeController();
        Map = new MapData();    
        StageAct = 0;

        if (GameManager.SaveManager.SaveFileCheck())
            GameManager.SaveManager.LoadGame();
    }

    public void MainDeckSet()
    {
        GameData.Incarna = GameDataMain.Incarna;
        GameData.DarkEssence = GameDataMain.DarkEssence;
        GameData.PlayerHP = GameDataMain.PlayerHP;
        GameData.DeckUnits = GameDataMain.DeckUnits;
        GameData.FallenUnits = GameDataMain.FallenUnits;
        GameData.Progress.ClearProgress();
    }

    public void DeckClear()
    {
        GameData.Incarna = GameDataTutorial.Incarna;
        GameData.DarkEssence = GameDataTutorial.DarkEssence;
        GameData.PlayerHP = GameDataTutorial.PlayerHP;
        GameData.DeckUnits = GameDataTutorial.DeckUnits;
        GameData.FallenUnits.Clear();
        GameData.Progress.ClearProgress();
        GameData.StageDivine = new();

        //GameData.npcQuest.ClearQuest();
        GameData.CurrentAct = 1;

        foreach (DeckUnit unit in GameData.DeckUnits)
        {
            unit.DeckUnitChangedStat.ClearStat();
            unit.DeckUnitUpgradeStat.ClearStat();
            unit.ClearStigma();
        }
    }

    //디버깅용 메서드
    public void MainDeckLayoutSet()
    {
        GameDataMain.Incarna = GameDataMainLayout.Incarna;
        GameDataMain.DarkEssence = GameDataMainLayout.DarkEssence;
        GameDataMain.PlayerHP = GameDataMainLayout.PlayerHP;
        GameDataMain.DeckUnits = GameDataMainLayout.DeckUnits;
        GameDataMain.FallenUnits = GameDataMainLayout.FallenUnits;
        GameDataMain.Progress.ClearProgress();

        foreach (DeckUnit unit in GameDataMain.DeckUnits)
        {
            unit.DeckUnitChangedStat.ClearStat();
            unit.DeckUnitUpgradeStat.ClearStat();
            unit.ClearStigma();
        }
    }

    public int GetHallUnitID()
    {
        int hallUnitID = -1;
        List<HallUnit> hallUnits = GameManager.OutGameData.FindHallUnitList();

        for (int i = 4; i < Mathf.Infinity; i++)
        {
            if (hallUnits.Find(x => x.ID == i) == null)
            {
                hallUnitID = i;
                break;
            }
        }

        return hallUnitID;
    }

    public void HallDeckSet()
    {
        GameDataMain.DeckUnits.Clear();
        GameManager.OutGameData.DataIntegrityCheck();
        foreach (var hallUnit in GameManager.OutGameData.FindHallUnitList())
        {
            if (hallUnit.IsMainDeck)
            {
                GameDataMain.DeckUnits.Add(hallUnit.ConvertToDeckUnit());
            }
        }
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    T LoadJson<T>(string path)
    {
        TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"Data/{path}");

        return JsonUtility.FromJson<T>(textAsset.text);
    }

    [SerializeField] private List<DeckUnit> _playerDeck = new ();
    public List<DeckUnit> PlayerDeck => _playerDeck;

    public void AddDeckUnit(DeckUnit unit)
    {
        GameData.DeckUnits.Add(unit);
    }

    public void RemoveDeckUnit(DeckUnit unit)
    {
        GameData.DeckUnits.Remove(unit);
    }

    public List<DeckUnit> GetDeck()
    {
        return GameData.DeckUnits;
    }

    public List<DeckUnit> GetSortedDeck(SortMode sortMode)
    {
        List<DeckUnit> returnUnits = new List<DeckUnit>();

        // 정렬 방식은 SortMode Enum 참고

        switch (sortMode)
        {
            case SortMode.Hall:
                List<DeckUnit> mainDecks = GameData.DeckUnits.FindAll(x => x.IsMainDeck);
                List<DeckUnit> notMainDecks = GameData.DeckUnits.FindAll(x => !x.IsMainDeck);

                mainDecks.Sort((a, b) => a.HallUnitID.CompareTo(b.HallUnitID));
                notMainDecks.Sort((a, b) => b.GetStigmaCount().CompareTo(a.GetStigmaCount()));

                returnUnits.AddRange(mainDecks);
                returnUnits.AddRange(notMainDecks);
                break;

            case SortMode.HP:
                returnUnits = GameData.DeckUnits;
                returnUnits.Sort((a, b) => b.DeckUnitTotalStat.MaxHP.CompareTo(a.DeckUnitTotalStat.MaxHP));

                foreach (DeckUnit unit in returnUnits)
                {
                    Debug.Log($"{unit.Data.Name} : {unit.DeckUnitTotalStat.MaxHP}");
                }
                break;

            case SortMode.Attack:
                returnUnits = GameData.DeckUnits;
                returnUnits.Sort((a, b) => b.DeckUnitTotalStat.ATK.CompareTo(a.DeckUnitTotalStat.ATK));

                foreach (DeckUnit unit in returnUnits)
                {
                    Debug.Log($"{unit.Data.Name} : {unit.DeckUnitTotalStat.ATK}");
                }
                break;

            case SortMode.Speed:
                returnUnits = GameData.DeckUnits;
                returnUnits.Sort((a, b) => b.DeckUnitTotalStat.SPD.CompareTo(a.DeckUnitTotalStat.SPD));

                foreach (DeckUnit unit in returnUnits)
                {
                    Debug.Log($"{unit.Data.Name} : {unit.DeckUnitTotalStat.SPD}");
                }
                break;

            case SortMode.Cost:
                returnUnits = GameData.DeckUnits;
                returnUnits.Sort((a, b) => a.DeckUnitTotalStat.ManaCost.CompareTo(b.DeckUnitTotalStat.ManaCost));

                foreach (DeckUnit unit in returnUnits)
                {
                    Debug.Log($"{unit.Data.Name} : {unit.DeckUnitTotalStat.ManaCost}");
                }
                break;

            default:
                returnUnits = GameData.DeckUnits;
                returnUnits.Sort((a, b) =>
                {
                    if (a.Data.Rarity == b.Data.Rarity)
                        return b.GetStigmaCount().CompareTo(a.GetStigmaCount());
                    else 
                        return b.Data.Rarity.CompareTo(a.Data.Rarity);
                });
                break;
        }

        return returnUnits;
    }

    public void SetDeck(List<DeckUnit> deck)
    {
        GameData.DeckUnits = deck;
    }

    public bool DarkEssenseChage(int cost)
    {
        if (GameData.DarkEssence + cost < 0)
        {
            return false;
        }
        else
        {
            GameData.DarkEssence += cost;
            return true;
        }
    }

    public bool CanUseDarkEssense(int value)
    {
        if (GameData.DarkEssence >= value)
            return true;

        Debug.Log("not enough Dark Essense");
        return false;
    }

    public List<PlayerSkill> GetPlayerSkillList()
    {
        List<PlayerSkill> skillList = new ();

        foreach (PlayerSkill skill in GameData.Incarna.PlayerSkillList)
        {
            SetSkillCost(skill);
            skillList.Add(skill);
        }

        return skillList;
    }

    public void SetSkillCost(PlayerSkill playerSkill)
    {
        ChangeSkillCost(playerSkill, 1, 53, 5, 0);
        ChangeSkillCost(playerSkill, 3, 52, 0, 1);
        ChangeSkillCost(playerSkill, 5, 63, 0, 1);
        ChangeSkillCost(playerSkill, 7, 73, 10, 0);
    }

    public void ChangeSkillCost(PlayerSkill playerSkill, int ID, int shopID, int mana, int darkessence)
    {
        int tmpMana = playerSkill.GetOriginalManaCost() - mana;
        int tmpDarkEssence = playerSkill.GetOriginalDarkEssenceCost() - darkessence;

        if (playerSkill.GetID() == ID)
        {
            if (GameManager.OutGameData.IsUnlockedItem(shopID))
            {
                playerSkill.ChangeCost(tmpMana, tmpDarkEssence);
            }
            else
            {
                playerSkill.ChangeCost(playerSkill.GetOriginalManaCost(), playerSkill.GetOriginalDarkEssenceCost());
            }
        }
    }

    public MapDataSaveData GetMapSaveData()
    {
        MapDataSaveData saveData = new();

        saveData.MapObject = GameManager.Data.Map.MapObject.name;
        saveData.StageList = GameManager.Data.Map.StageList;
        saveData.CurrentTileID = GameManager.Data.Map.CurrentTileID;
        saveData.ClearTileID = GameManager.Data.Map.ClearTileID;

        return saveData;
    }

    public void SetMapSaveData(MapDataSaveData saveData)
    {
        string mapName = saveData.MapObject;

        if (mapName == "TutorialMap")
        {
            Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/TutorialMap/TutorialMap");
        }
        else if (mapName.StartsWith("Map1_"))
        {
            Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/StageAct0/" + mapName);
        }
        else if (mapName.StartsWith("Map2_"))
        {
            Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/StageAct1/" + mapName);
        }
        else if (mapName.StartsWith("Map3_"))
        {
            Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/StageAct2/" + mapName);
        }

        Map.StageList = saveData.StageList;
        Map.CurrentTileID = saveData.CurrentTileID;
        Map.ClearTileID = saveData.ClearTileID;
    }
}