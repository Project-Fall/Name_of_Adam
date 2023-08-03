using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ����ȭ�� �ӽ� Ŭ����
[Serializable]
public class TestStageList
{
    public TestStageContainer[] MapList = new TestStageContainer[5];
}
[Serializable]
public class TestStageContainer
{
    public StageType Type;
    public StageName Stage;
    public List<int> Branch;
}

[Serializable]
public class MapData
{
    public List<TestStage> StageList;
    public int CurrentTileID;
    // public int[] PassedTileID; // ������ Ÿ��(��ȹ�� ���� �ٸ���)
}

public class TestStageManager : MonoBehaviour
{
    private static TestStageManager s_instance;
    public static TestStageManager Instance { get { Init(); return s_instance; } }

    List<TestStage> StageList;
    [SerializeField] TestStage CurrentStage;
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
            s_instance = go.GetComponent<TestStageManager>();
        }
    }

    public void InputStageList(TestStage stage)
    {
        if (StageList == null)
            StageList = new List<TestStage>();
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

        foreach(TestStage value in StageList)
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
        foreach (TestStage stage in CurrentStage.NextStage)
            stage.StartBlink();
    }

    public void StageMove(int _id)
    {
        _stageChanger.SetNextStage(_id);
    }
}

// StageManger���� ���������� �����ϰ� �����ϴ� �������
// ���ŷ����� �����տ� Ŭ�� �� ��ȣ�ۿ��� �ϴ� ��ũ��Ʈ�� �ֱ�(Action���� �ż��带 �ޱ⸸ �ϴ°ɷ� �ϴ°� ���� ��)
// ���������� ����°� ������? ����? ���پ�? ��常?