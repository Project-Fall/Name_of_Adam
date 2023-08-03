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

// StageManger���� ���������� �����ϰ� �����ϴ� �������
// ���ŷ����� �����տ� Ŭ�� �� ��ȣ�ۿ��� �ϴ� ��ũ��Ʈ�� �ֱ�(Action���� �ż��带 �ޱ⸸ �ϴ°ɷ� �ϴ°� ���� ��)
// ���������� ����°� ������? ����? ���پ�? ��常?