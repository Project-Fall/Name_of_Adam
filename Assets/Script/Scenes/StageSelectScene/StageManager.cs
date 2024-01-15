using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class MapData
{
    public GameObject MapObject;      // 현재 맵 구조의 프리팹
    public List<StageData> StageList; // 맵의 노드들의 리스트
    public int CurrentTileID;         // 현재 위치하고있는 타일의 ID
    public List<int> ClearTileID;     // 이미 클리어한 타일의 ID

    public StageData GetStage(int ID) => StageList.Find(x => x.ID == ID);
    public StageData GetCurrentStage() => StageList.Find(x => x.ID == CurrentTileID);
}

public class StageManager : MonoBehaviour
{
    private static StageManager s_instance;
    public static StageManager Instance { get { Init(); return s_instance; } }

    [SerializeField] StageCameraController CameraController;
    StageChanger _stageChanger;

    List<Stage> StageList;
    public Stage CurrentStage;


    private void Awake()
    {
        _stageChanger = new StageChanger();

        ActClearCheck();

        // 맵 프리팹이 존재하지 않다면(스테이지 최초 진입 시) 맵중에 랜덤으로 하나 가져오기
        if (GameManager.Data.Map.MapObject == null)
        {
            if (GameManager.Data.StageAct == 0)
            {
                GameManager.Data.Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/TutorialMap");
            }
            else if (GameManager.Data.StageAct == 1)
            {
                GameManager.Data.Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/VideoMap1");
                //GameManager.Data.Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/ProtoMap1");
            }
            else if (GameManager.Data.StageAct == 2)
            {
                GameManager.Data.Map.MapObject = Resources.Load<GameObject>("Prefabs/Stage/Maps/VideoMap2");
            }
            else
            {
                GameObject[] maps = Resources.LoadAll<GameObject>("Prefabs/Stage/Maps");
                GameManager.Data.Map.MapObject = maps[UnityEngine.Random.Range(0, maps.Length)];
            }
        }
        Instantiate(GameManager.Data.Map.MapObject);
    }

    private void Start()
    {
        if (GameManager.Data.Map.StageList == null)
            SetStageData();
        SetCurrentStage();
        GameManager.VisualEffect.StartFadeEffect(true);
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@StageManager");
            s_instance = go.GetComponent<StageManager>();
        }
    }

    public void ActClearCheck()
    {
        // 마지막 스테이지일 때(ID가 99일 때)
        // 마지막 스테이지 조건을 보스일 때로 하고싶지만, 튜토리얼 스테이지의 경우에 의해 보스일 때로는 하지 못함
        if (GameManager.Data.Map.CurrentTileID == 99)
        {
            if (GameManager.Data.StageAct < 2) // 여기의 상수는 최대 막의 수, 지금은 1막밖에 없기에 1임
            {
                GameManager.Data.StageAct++;
                GameManager.Data.Map = new MapData();
                /*
                if (GameManager.Data.StageAct == 1) // 1막일 때(튜토리얼 클리어, 게임 시작) 기본 덱으로 세팅
                {
                    GameManager.Data.MainDeckSet();

                    GameManager.Data.GameData.FallenUnits.Clear();
                    GameManager.Data.GameData.FallenUnits.AddRange(GameManager.Data.GameDataMain.DeckUnits);
                }
                */
            }
            else
            {
                // 게임 클리어
            }
        }
    }

    public void InputStageList(Stage stage)
    {
        if (StageList == null)
            StageList = new List<Stage>();
        if(!StageList.Contains(stage))
            StageList.Add(stage);
    }

    public void SetCurrentStage()
    {
        int curID = GameManager.Data.Map.CurrentTileID;
        CurrentStage = StageList.Find(x => x.Datas.ID == curID);
        StartCoroutine(CurrentStage.Fade());

        foreach (Stage st in CurrentStage.NextStage)
            st.SetNextStage();

        CameraController.SetLocate(CurrentStage.transform.localPosition.y + 2);
    }

    private void SetStageData()
    {
        List<StageData> StageDatas = new List<StageData>();
        //int maxLevel = 2; // 한 막에 몇가지 레벨이 존재하느냐에 따라 값이 증가함. ex) 한 맵에 3가지 레벨이 존재한다 -> maxLevel = 3
        //int addLevel = (GameManager.Data.StageAct - 1) * maxLevel;
        List<Vector2> existStage = new List<Vector2>();

        for (int i = 0; i < StageList.Count; i++)
        {
            /*
            if (StageList[i].Datas.Type == StageType.Battle && StageList[i].Datas.Name == StageName.CommonBattle)
            {
                //12/20 프로토타입 버전은 레벨별 데이터 차이 없음, 추후 사용 가능성 다수
                //int x = (StageList[i].Datas.ID <= 1 && addLevel == 0) ? 1 : (int)StageList[i].Datas.StageLevel + addLevel;
                int x = GameManager.Data.StageAct + 1;
                int y = UnityEngine.Random.Range(0, GameManager.Data.StageDatas[x].Count);

                Vector2 vec = new Vector2(x, y);

                if (!existStage.Contains(vec))
                {
                    existStage.Add(vec);
                    StageDatas.Add(StageList[i].SetBattleStage(x, y));
                }
                else
                    i--;
            }
            else
            {
                StageDatas.Add(StageList[i].Datas);
            }
            */
            StageDatas.Add(StageList[i].Datas);
        }

        GameManager.Data.Map.StageList = StageDatas;
    }

    public void StageMove(int id)
    {
        foreach (Stage st in CurrentStage.NextStage)
        {
            if (st.Datas.ID == id)
            {
                GameManager.VisualEffect.StartFadeEffect(false);
                PlayAfterCoroutine(() => _stageChanger.SetNextStage(id), 0.8f);
            }
        }
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));

    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}