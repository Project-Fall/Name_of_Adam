using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HarlotSceneController : MonoBehaviour,StigmaInterface
{
    private readonly int[] enterDialogNums = { 3, 3, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    private DeckUnit _stigmatizeUnit;
    private List<Stigma> _stigmaList; //타락낙인 저장하는 곳
    private List<DeckUnit> _RestorationUnits;

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject fall_background;
    [SerializeField] private Image foogyImg;

    [SerializeField] private GameObject _SelectStigmaButton;
    [SerializeField] private GameObject _SelectStigmaButton_disabled;
    [SerializeField] private GameObject _getOriginUnitButton;
    [SerializeField] private GameObject _getOriginUnitButton_disabled;
    [SerializeField] private GameObject _ui_SelectMenu;
    [SerializeField] private List<DeckUnit> _originUnits = null;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private TMP_Text unitRestoration_txt;
    [SerializeField] private TMP_Text originUnit_txt;
    [SerializeField] private TMP_Text selectStigma_txt;

    [SerializeField] private Button _forbiddenButton; // 접근 금지 버튼

    private List<Script> _scripts = null;
    private bool _isStigmaFull;
    private bool _isNPCFall = false;
    private UI_Conversation _conversationUI;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _scripts = new List<Script>();
        _isStigmaFull = false;
        _RestorationUnits = new List<DeckUnit>();

        Debug.Log($"횟수: {GameManager.Data.GameData.NpcQuest.DarkshopQuest}");

        int questLevel = (int)(GameManager.Data.GameData.NpcQuest.DarkshopQuest / 7.5f);
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitDarkshop() == false && questLevel != 4)
        {
            _scripts = GameManager.Data.ScriptData["탕녀_입장_최초"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["탕녀_선택_0"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["탕녀_선택_0"][0].name));
            //GameManager.OutGameData.setVisitDarkshop(true);
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"탕녀_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"탕녀_선택_{25 * questLevel}"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"탕녀_선택_{25 * questLevel}"][0].name));

            if (questLevel == 4)
            {   
                background.SetActive(false);
                fall_background.SetActive(true);
                _isNPCFall = true;
            }
            else if (questLevel >= 0)
            {
                Color color = this.foogyImg.color;
                color.a = questLevel * 0.25f;
                this.foogyImg.color = color;
            }
        }

        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
        _conversationUI = FindObjectOfType<UI_Conversation>();
        _conversationUI.ConversationEnded += OnConversationEnded;

        _stigmaList = GameManager.Data.StigmaController.GetHarlotStigmaList();

        int current_DarkEssense = GameManager.Data.DarkEssense;

        if (!GameManager.OutGameData.IsUnlockedItem(5))
        {
            _getOriginUnitButton.SetActive(false);
        }
        else if (current_DarkEssense < ((_isNPCFall) ? 8 : 10)) 
        {
            _getOriginUnitButton_disabled.SetActive(true);
            _getOriginUnitButton.SetActive(false);
        }

        if (current_DarkEssense < ((_isNPCFall) ? 8 : 10))
        {
            _SelectStigmaButton_disabled.SetActive(true);
            _SelectStigmaButton.SetActive(false);
        }
        SetMenuText(_isNPCFall);
        /*
        if(GameManager.Data.GameData)
        {
            _forbiddenButton.gameObject.SetActive(true);
        }
        else
        {
            _forbiddenButton.gameObject.SetActive(false);
        }
        */
    }
    //오리지날 유닛 연성하기
    public void OnMakeOriginUnitClick()
    {
        //연출하기 애니매이션 끝나면 
        //Debug.Log(GameManager.Data.GameData.DeckUnits);
        GameManager.Sound.Play("UI/ClickSFX/UIClick2");

        int unitidx = Random.Range(0, 3);
        UI_UnitInfo ui = GameManager.UI.ShowPopup<UI_UnitInfo>();

        ui.SetUnit(_originUnits[unitidx]);
        ui.Init(OnSelectMakeUnit, CUR_EVENT.COMPLETE_HAELOT, OnQuitClick);

    }
    public void OnSelectMakeUnit(DeckUnit unit)
    {
        GameManager.Data.AddDeckUnit(unit);
        GameManager.Data.DarkEssenseChage(((_isNPCFall) ? -8 : -10));
        GameManager.Data.GameData.FallenUnits.Add(unit);
    }

    //유닛을 검은 정수로 환원하는 버튼
    public void OnUnitRestorationClick()
    {
        _RestorationUnits.Clear();
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        UI_MyDeck ui = GameManager.UI.ShowPopup<UI_MyDeck>();
        ui.Init(false, OnSelectRestoration, CUR_EVENT.HARLOT_RESTORATION, RestorationQuitClick);
        ui.SetEventMenu(_ui_SelectMenu);
    }

    public void OnSelectRestoration(DeckUnit unit)
    {
        if (!_RestorationUnits.Contains(unit))
            _RestorationUnits.Add(unit);
        else
            _RestorationUnits.Remove(unit);

        GameManager.UI.ClosePopup();
    }

    // 유닛 선택 후 타락 관련 낙인 부여 버튼
    public void OnStigmaButtonClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        UI_MyDeck ui = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ui.Init(false, OnSelectStigmatization, CUR_EVENT.STIGMA);
        ui.SetEventMenu(_ui_SelectMenu);
    }
    public void IsStigmaFull()
    {
        Debug.Log("스티그마 꽉 찼을 때 예외처리");
        _isStigmaFull = true;
        GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(null, "Full Stigma", _stigmatizeUnit.GetStigma(true), 0, null, this);
    }

    public void OnSelectStigmatization(DeckUnit unit)
    {
        _stigmatizeUnit = unit;

        if (_stigmatizeUnit.GetStigmaCount() < _stigmatizeUnit._maxStigmaCount)
        {
            Debug.Log("유닛이 스티그마를 더 받을 수 잇는 상태입니다.");
            //GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(_stigmatizeUnit, null, 3, null, this);
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(_stigmatizeUnit, "Select Stigma", _stigmaList, 2, null, this);
        }
        else
        {
            Debug.Log("유닛 스티그마 더 받을 수 없으니 하나를 선택하여 지워야 합니다.");
            IsStigmaFull();
        }
        GameManager.Data.DarkEssenseChage(((_isNPCFall) ? -8 : -10));
    }

    private void SetUnitStigma(Stigma stigma)
    {
        _stigmatizeUnit.AddStigma(stigma);

        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        UI_UnitInfo ui = GameManager.UI.ShowPopup<UI_UnitInfo>();
        ui.SetUnit(_stigmatizeUnit);
        ui.Init(null, CUR_EVENT.COMPLETE_STIGMA, OnQuitClick);
    }

    public void OnStigmaSelected(Stigma stigma)
    {
        if (_isStigmaFull)//스티그마 예외처리
        {
            _isStigmaFull = false;
            _stigmatizeUnit.DeleteStigma(stigma);
            GameManager.UI.CloseAllPopup();
            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_stigmatizeUnit);
            unitInfo.Init(OnSelectStigmatization, CUR_EVENT.STIGMA_EXCEPTION);
            return;
        }
        SetUnitStigma(stigma);
    }

    //대화하기 버튼을 클릭했을 경우
    public void OnConversationButtonClick()
    {
        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
    }

    //나가기 버튼을 클릭했을 경우
    public void RestorationQuitClick()
    {
        if (_RestorationUnits.Count == GameManager.Data.GetDeck().Count)
        {
            Debug.Log("유닛은 하나 이상 남겨야 합니다.");//여기다가 경고창 띄우면 될듯
            return;
        }
        else if (_RestorationUnits.Count != 0)
        {
            int cost = _isNPCFall ? 2 * _RestorationUnits.Count : _RestorationUnits.Count;
            GameManager.Data.DarkEssenseChage(cost);
            foreach (DeckUnit delunit in _RestorationUnits)
                GameManager.Data.RemoveDeckUnit(delunit);
        }

        GameManager.UI.ClosePopup();
        OnQuitClick();
    } 

    public void OnQuitClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/BackButtonClickSFX");
        StartCoroutine(QuitScene());
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (GameManager.Data.GameData.IsVisitDarkShop == false)
        {
            GameManager.Data.GameData.IsVisitDarkShop = true;
        }
        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        int questLevel = (int)(GameManager.Data.GameData.NpcQuest.DarkshopQuest / 7.5f);
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitDarkshop() == false && questLevel < 4)
        {
            GameManager.OutGameData.SetVisitDarkshop(true);
            quitScript.Init(GameManager.Data.ScriptData["탕녀_퇴장_최초"], false);
        }
        else 
        {

            quitScript.Init(GameManager.Data.ScriptData[$"탕녀_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
        }
        
        yield return StartCoroutine(quitScript.PrintScript());
        GameManager.Data.Map.ClearTileID.Add(GameManager.Data.Map.CurrentTileID);
        GameManager.SaveManager.SaveGame();
        SceneChanger.SceneChange("StageSelectScene");
    }
    private void SetMenuText(bool _isnpcfall)
    {
        if (_isnpcfall)
        {
            unitRestoration_txt.text = "2";
            selectStigma_txt.text = "8";
            originUnit_txt.text = "8";
        }
        else
        {
            unitRestoration_txt.text = "1";
            selectStigma_txt.text = "10";
            originUnit_txt.text = "10";
        }
    }
    private void OnConversationEnded()
    {
        _ui_SelectMenu.SetActive(true);
    }
}
