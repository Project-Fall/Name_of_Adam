using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 직렬화용 임시 클래스
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

public class TestStageManager : MonoBehaviour
{
    private static TestStageManager instance;
    public static TestStageManager Instance => instance;

    StageChanger _stageChanger;

    [SerializeField] TestStage CurrentStage;


    private void Awake()
    {
        instance = this;
        _stageChanger = new StageChanger();
    }

    private void Start()
    {
        StartBlink();
    }


    private void StartBlink()
    {
        foreach (TestStage stage in CurrentStage.NextStage)
            stage.StartBlink();
    }

    public void StageMove(TestStage _st)
    {
        Stage stage = new Stage(_st.Stage, _st.Type, 0, 0, null);

        _stageChanger.SetNextStage(stage);
    }
}

// StageManger에서 프리팹으로 생성하고 관리하는 방법으로
// 번거롭지만 프리팹에 클릭 시 상호작용을 하는 스크립트를 넣기(Action으로 매서드를 받기만 하는걸로 하는게 좋을 듯)
// 프리팹으로 만드는건 어디까지? 전부? 한줄씩? 노드만?