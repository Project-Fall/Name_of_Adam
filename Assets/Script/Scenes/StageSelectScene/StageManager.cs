using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[Serializable]
public class MapData
{
    public GameObject MapObject;      // 현재 맵 구조의 프리팹
    public MapDataContainer MapDataContainer; //프리팹의 클래스
    public List<StageData> StageList; // 맵의 노드들의 리스트
    public int CurrentTileID;         // 현재 위치하고있는 타일의 ID
    public List<int> ClearTileID;     // 이미 클리어한 타일의 ID

    public StageData GetStage(int ID) => StageList.Find(x => x.ID == ID);
    public StageData GetCurrentStage() => StageList.Find(x => x.ID == CurrentTileID);
    public void SetCurrentTileClear()
    {
        if (!ClearTileID.Contains(CurrentTileID)) 
            ClearTileID.Add(CurrentTileID);
    }
   
}

public class StageManager : MonoBehaviour
{
    private static StageManager s_instance;
    public static StageManager Instance { get { if (s_instance == null) { Init(); } return s_instance; } }

    [SerializeField] public StageCameraController CameraController;

    [SerializeField] private GameObject _yohrnBackground;
    [SerializeField] private GameObject _saviorBackground;
    [SerializeField] private GameObject _phanuelBackground;
    [SerializeField] private GameObject _defaultBackground;

    [SerializeField] private TextMeshProUGUI _chapterText;

    private StageChanger _stageChanger;

    private List<Stage> _stageList = new();
    public Stage CurrentStage;

    private bool _isClicked = false;

    private static void Init()
    {
        GameObject go = GameObject.Find("@StageManager");
        s_instance = go.GetComponent<StageManager>();
    }

    private void Start()
    {
        _stageChanger = new StageChanger();

        ActClearCheck(); // 만약 클리어 체크가 될 시 Map이 초기화됨

        if (GameManager.Data.Map.MapObject == null)
            LoadMapData();

        Debug.Log(GameManager.Data.Map.MapObject);

        GameManager.Data.Map.MapDataContainer = Instantiate(GameManager.Data.Map.MapObject).GetComponent<MapDataContainer>();

        GameManager.Data.Map.MapDataContainer.transform.localScale = new(0.8f, 0.8f, 0.8f);

        foreach (Stage stage in GameManager.Data.Map.MapDataContainer.MapStageList)
        {
            if (_stageList.Contains(stage))
                continue;
            _stageList.Add(stage);
        }

        if (GameManager.Data.Map.ClearTileID == null)
            GameManager.Data.Map.ClearTileID = new();

        if (GameManager.Data.Map.StageList == null)
        {
            SetStageData();
            foreach (Stage stage in GameManager.Data.Map.MapDataContainer.MapStageList)
            {
                stage.Init();
            }
        }
        else
        {
            foreach (Stage stage in GameManager.Data.Map.MapDataContainer.MapStageList)
            {
                StageData data = GameManager.Data.Map.StageList.Find(s => s.ID == stage.Datas.ID);
                stage.SetBattleStage(data.StageLevel, data.StageID);
                stage.Init();
            }
        }

        SetCurrentStage();

        GameManager.VisualEffect.StartFadeEffect(true);
        SetBackground();
        SetChapterText();
    }

    private void SetChapterText()
    {
        string chapter = "";
        if (GameManager.Data.GameData.CurrentAct == 99)
            chapter += "Trial";
        else
            chapter += "Act " + GameManager.Data.GameData.CurrentAct.ToString();
        
        chapter += ". ";

        chapter += "Chapter " + (GameManager.Data.StageAct + 1).ToString() + ".";

        _chapterText.SetText(chapter);
    }

    private void SetBackground()
    {
        if (GameManager.Data.StageAct == 2)
        {
            StageData data = GameManager.Data.Map.GetStage(99);
            string unitName = GameManager.Data.StageDatas[data.StageLevel][data.StageID].Units[0].Name;

            _phanuelBackground.SetActive(unitName == "바누엘");
            _saviorBackground.SetActive(unitName == "구원자");
            _yohrnBackground.SetActive(unitName == "욘");
        }
        else
        {
            _defaultBackground.SetActive(true);
        }
    }
    private void LoadMapData()
    {
        string mapFolderPath = "Prefabs/Stage/Maps/";

        if (GameManager.Data.StageAct == 0 && !GameManager.OutGameData.Data.TutorialClear)
        {
            mapFolderPath += "TutorialMap";
        }
        else
        {
            mapFolderPath += "StageAct" + GameManager.Data.StageAct;
        }

        GameObject[] maps = Resources.LoadAll<GameObject>(mapFolderPath);
        GameManager.Data.Map.MapObject = maps[UnityEngine.Random.Range(0, maps.Length)];
    }


    public void ActClearCheck()
    {
        // 마지막 스테이지일 때(ID가 99일 때)
        // 마지막 스테이지 조건을 보스일 때로 하고싶지만, 튜토리얼 스테이지의 경우에 의해 보스일 때로는 하지 못함
        if (GameManager.Data.Map.CurrentTileID == 99 && GameManager.Data.StageAct < 2)
        {
            GameManager.Data.StageAct++;
            GameManager.Data.Map = new MapData();
        }
    }

    public void SetCurrentStage()
    {
        int curID = GameManager.Data.Map.CurrentTileID;
        CurrentStage = _stageList.Find(x => x.Datas.ID == curID);
        StartCoroutine(CurrentStage.Fade());

        foreach (Stage stage in CurrentStage.NextStage)
        {
            if (stage != null)
                stage.BackLight.Blink();
        }
         _isClicked = false;

        CameraController.SetLocate(CurrentStage.transform.localPosition.y + 2);
    }

    private void SetStageData()
    {
        /*
        0: Tutorial
        1: Act 1 First half
        2: Act 1 Last Half
        3: Act 2
        4: Act 3

        0: Tutorial
        10: Chapter 1
        20: Chapter 2
        30: Chapter 3
        90: Endless

        0xx Common Battle
        100 Elite Battle
        200 Boss Battle
        */
        List<StageData> stageDataList = new();
        List<(int, int)> existStage = new();

        foreach (Stage stage in _stageList)
        {
            StageData stageData = stage.Datas;
            if (stageData.Type == StageType.Battle)
            {
                int stageLevel = 0;
                int stageID = 0;

                //현재 Act를 통해 stageLevel을 결정, (1의 자리)
                switch (GameManager.Data.StageAct)
                {
                    case 0:
                        stageLevel = stageData.ID < 4 ? 1 : 2;
                        break;
                    case 1:
                        stageLevel = 3;
                        break;
                    case 2:
                        stageLevel = 4;
                        break;
                }

                //현재 Chapter를 통해 stageLevel을 결정, (10의 자리)
                if (GameManager.Data.GameData.CurrentAct == 99)
                {
                    stageLevel += 90;
                }
                else
                {
                    stageLevel += 10 * GameManager.Data.GameData.CurrentAct;
                }

                //현재 전투 스테이지를 통해 stageLevel와 stageID을 결정, (100의 자리)
                if (stageData.Name == StageName.CommonBattle)
                {
                }
                else if (stageData.Name == StageName.EliteBattle)
                {
                    stageLevel += 100;
                }
                else if (stageData.Name == StageName.BossBattle)
                {
                    stageLevel += 200;
                }

                while (true)
                {
                    int randNum = UnityEngine.Random.Range(0, GameManager.Data.StageDatas[stageLevel].Count);
                    if (!existStage.Contains((stageLevel, randNum)))
                    {
                        stageID = randNum;
                        break;
                    }
                }

                existStage.Add((stageLevel, stageID));
                stageDataList.Add(stage.SetBattleStage(stageLevel, stageID));
            }
            else
            {
                stageDataList.Add(stageData);
            }
        }

        GameManager.Data.Map.StageList = stageDataList;

    }

    public void StageMove(int id)
    {
        if (_isClicked)
            return;

        foreach (Stage st in CurrentStage.NextStage)
        {
            if (st != null && st.Datas.ID == id)
            {
                _isClicked = true;

                GameManager.Sound.Clear();
                GameManager.Sound.Play("UI/UISFX/UISelectSFX");
                GameManager.VisualEffect.StartFadeEffect(false);
                PlayAfterCoroutine(() => _stageChanger.SetNextStage(id), 0.4f);
            }
        }
    }

    public void StageMouseClick(Stage stage)
    {
        if (GameManager.Data.StageAct == 1)
        {
            return;
        }

        GameManager.SaveManager.SaveGame();
        StageManager.Instance.StageMove(stage.Datas.ID);

        _isHover = false;

        if (_isHoverMessegeOn)
        {
            _isHoverMessegeOn = false;
            GameManager.UI.CloseHover();
        }
    }

    private bool _isHover = false;
    private bool _isHoverMessegeOn = false;
    private Stage _hoverStage;

    public void StageMouseEnter(Stage stage)
    {
        _isHover = true;
        _hoverStage = stage;
        PlayAfterCoroutine(() => {
            if (_isHover && !_isHoverMessegeOn && _hoverStage == stage)
            {
                _isHoverMessegeOn = true;
                GameManager.UI.ShowHover<UI_TextHover>().SetText(
                    $"{GameManager.Locale.GetLocalizedSelectStageScene(stage.Datas.Name.ToString())}", Input.mousePosition);
            }
        }, 0.5f);
        
        if (CurrentStage.NextStage.Contains(stage))
        {
            foreach (Stage st in CurrentStage.NextStage)
            {
                if (st != stage && st != null)
                {
                    st.BackLight.FadeOut();
                }
                else
                {
                    stage.BackLight.FadeIn();
                }
            }
            return;
        }

        if (!GameManager.Data.Map.ClearTileID.Contains(stage.Datas.ID))
        {
            stage.BackLight.FadeIn();
        }
    }

    public void StageMouseExit(Stage stage)
    {
        _isHover = false;

        if (_isHoverMessegeOn)
        {
            _isHoverMessegeOn = false;
            GameManager.UI.CloseHover();
        }

        if (CurrentStage.NextStage.Contains(stage))
        {
            foreach (Stage st in StageManager.Instance.CurrentStage.NextStage)
            {
                if (st != null)
                {
                    st.BackLight.Blink();
                }
            }
            return;
        }

        if (!GameManager.Data.Map.ClearTileID.Contains(stage.Datas.ID))
        {
            stage.BackLight.FadeOut();
        }
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));

    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}