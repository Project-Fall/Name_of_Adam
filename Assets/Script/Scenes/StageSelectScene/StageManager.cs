using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapData
{
    public List<Stage> StageList;
    public int CurrentTileID;
    // public int[] PassedTileID;
}

public class StageManager : MonoBehaviour
{
    private static StageManager s_instance;
    public static StageManager Instance { get { Init(); return s_instance; } }

    List<Stage> StageList;
    [SerializeField] Stage CurrentStage;

    StageChanger _stageChanger;


    private void Awake()
    {
        _stageChanger = new StageChanger();
    }

    private void Start()
    {
        if (GameManager.Data.Map.StageList == null)
            SetBattleLevel();
        SetCurrentStage();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@StageManager");
            s_instance = go.GetComponent<StageManager>();
        }
    }

    public void InputStageList(Stage stage)
    {
        if (StageList == null)
            StageList = new List<Stage>();
        StageList.Add(stage);
    }

    public void SetCurrentStage()
    {
        int curID = GameManager.Data.Map.CurrentTileID;
        CurrentStage = StageList.Find(x => x.ID == curID);
        StartBlink();
    }

    private void SetBattleLevel()
    {
        int addLevel = GameManager.Data.StageAct * 2;
        foreach (Stage value in StageList)
        {
            if (value.Type != StageType.Battle)
                continue;
            int x = (value.ID <= 1 && addLevel == 0) ? 0 : (int)value.BattleStageLevel + addLevel;
            int y = UnityEngine.Random.Range(0, GameManager.Data.StageDatas[x].Count);
            value.SetBattleStage(x, y);
        }
        GameManager.Data.Map.StageList = StageList;
    }

    private void StartBlink()
    {
        foreach (Stage stage in CurrentStage.NextStage)
            stage.StartBlink();
    }

    public void StageMove(int _id)
    {
        _stageChanger.SetNextStage(_id);
    }
}

// StageManger에서 프리팹으로 생성하고 관리하는 방법으로
// 번거롭지만 프리팹에 클릭 시 상호작용을 하는 스크립트를 넣기(Action으로 매서드를 받기만 하는걸로 하는게 좋을 듯)
// 프리팹으로 만드는건 어디까지? 전부? 한줄씩? 노드만?