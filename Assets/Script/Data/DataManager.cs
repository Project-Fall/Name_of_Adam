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

    public Dictionary<int, List<StageSpawnData>> StageDatas = new Dictionary<int, List<StageSpawnData>>();
    public MapData Map;
    public int StageAct;

    [SerializeField] public GameData GameData;
    [SerializeField] public GameData GameDataOriginal;

    public Dictionary<string, List<Script>> ScriptData = new Dictionary<string, List<Script>>();
    public StigmaController StigmaController;

    public void Init()
    {
        // StatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
        StageDatas = LoadJson<StageLoader, int, List<StageSpawnData>>("StageData").MakeDict();
        ScriptData = LoadJson<ScriptLoader, string, List<Script>>("Script").MakeDict();
        Map = new MapData();
        StigmaController = new StigmaController();

        _darkEssense = GameData.DarkEssence;
        _playerSkillCount = GameData.Incarna.PlayerSkillCount;
    }

    public void DeckClear()
    {
        GameData.Incarna = GameDataOriginal.Incarna;
        GameData.Money = GameDataOriginal.Money;
        GameData.DarkEssence = GameDataOriginal.DarkEssence;
        GameData.PlayerSkillCount = GameDataOriginal.PlayerSkillCount;
        GameData.DeckUnits = GameDataOriginal.DeckUnits;
        GameData.isVisitUpgrade = GameDataOriginal.isVisitUpgrade;
        GameData.isVisitStigma = GameDataOriginal.isVisitStigma;

        foreach (DeckUnit unit in GameData.DeckUnits)
        {
            unit.DeckUnitChangedStat.ClearStat();
            unit.DeckUnitUpgradeStat.ClearStat();
            unit.Stigma.Clear();
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

    public void SetDeck(List<DeckUnit> deck)
    {
        GameData.DeckUnits = deck;
    }

    private int _money;
    public int Money => _money;

    public bool MoneyChage(int cost)
    {
        if (_money + cost < 0)
        {
            return false;
        }
        else
        {
            _money += cost;
            return true;
        }
    }

    private int _darkEssense = 0;
    public int DarkEssense => _darkEssense;

    public bool DarkEssenseChage(int cost)
    {
        Debug.Log("Dark Essense: " + _darkEssense + " Change: " + cost);
        if (_darkEssense + cost < 0)
        {
            return false;
        }
        else
        {
            _darkEssense += cost;
            return true;
        }
    }

    public bool CanUseDarkEssense(int value)
    {
        if (_darkEssense >= value)
            return true;

        Debug.Log("not enough Dark Essense");
        return false;
    }

    private int _playerSkillCount = 0;
    public int PlayerSkillCount => _playerSkillCount;

    public bool PlayerSkillCountChage(int cost)
    {
        Debug.Log("Player Skill Count: " + _playerSkillCount + " Change: " + cost);
        if (_playerSkillCount + cost < 0)
        {
            return false;
        }
        else
        {
            _playerSkillCount += cost;
            return true;
        }
    }

    public List<PlayerSkill> GetPlayerSkillList()
    {
        List<PlayerSkill> skillList = new ();

        foreach (PlayerSkill skill in GameData.Incarna.PlayerSkillList)
        {
            skillList.Add(skill);
        }

        skillList.Insert(2, GameData.UniversalPlayerSkill);

        return skillList;
    }

    public List<int> GetProbability()
    {

        //원래는 이런 함수가 아니라 조건에 따라 확률이 바뀌어야함, 데이터 완성 시 적용
        //적어두겠음
        //90 9 1    ~1장 엘리트
        //80 15 5   ~1장 보스
        //70 20 10  ~2장 엘리트
        //60 25 15  ~2장 보스
        //하드에서는 4단계로 고정

        List<int> probability = new ();
        probability.Add(99);
        probability.Add(89);

        return probability;
    }
}
