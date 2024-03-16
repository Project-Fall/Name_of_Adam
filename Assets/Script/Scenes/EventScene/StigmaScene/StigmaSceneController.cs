using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StigmaSceneController : MonoBehaviour,StigmaInterface
{
    private readonly int[] enterDialogNums = { 3, 2, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    private DeckUnit _givestigmatizeUnit;
    private DeckUnit _stigmatizeUnit;
    private List<Script> _scripts;

    private bool _isStigmaFull = false;

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject fall_background;
    [SerializeField] private Image foogyImg;

    [SerializeField] private GameObject _stigma_transfer_btn = null;
    [SerializeField] private GameObject _stigma_transfer_btn_disabled;
    [SerializeField] private Button _forbiddenButton; // 접근 금지 버튼
    [SerializeField] private GameObject _ui_SelectMenu;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private UI_Conversation uiConversation;
    private Stigma _giveStigma = null;

    private bool _isNPCFall = false;

    void Start()
    {
        Init();
    }
    private void Init()
    {
        _scripts = new();
        _giveStigma = null;
        _isStigmaFull = false;

        //옮길 낙인유닛이 없다면 선택지가 안 뜨게
        bool isStigmaEmpty = true;
        List<DeckUnit> du = GameManager.Data.GetDeck();
        foreach(DeckUnit unit in du)
        {
            if (unit.GetStigma(true).Count != 0)
            {
                isStigmaEmpty = false;
                break;
            }
        }

        if (!GameManager.OutGameData.IsUnlockedItem(11))
        {
            _stigma_transfer_btn.SetActive(false);
        }
        else if (isStigmaEmpty)
        {
            _stigma_transfer_btn.SetActive(false);
            _stigma_transfer_btn_disabled.SetActive(true);
        }
        
        Debug.Log($"횟수: {GameManager.Data.GameData.NpcQuest.StigmaQuest}");

        int questLevel = (int)(GameManager.Data.GameData.NpcQuest.StigmaQuest / 12.5f);
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitStigma() == false && questLevel != 4)
        {
            //GameManager.OutGameData.setVisitStigma(true);
            _scripts = GameManager.Data.ScriptData["낙인소_입장_최초"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["낙인소_선택_0"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["낙인소_선택_0"][0].name));
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"낙인소_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"낙인소_선택_{25 * questLevel}"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"낙인소_선택_{25 * questLevel}"][0].name));

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
        uiConversation = FindObjectOfType<UI_Conversation>();
        uiConversation.ConversationEnded += OnConversationEnded;
    }

    // 낙인 부여를 선택했을 시
    public void OnStigmaUnitButtonClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        this._ui_SelectMenu.SetActive(false);
        UI_MyDeck ud = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ud.Init(false, OnSelectStigmatization, CUR_EVENT.STIGMA);
        ud.SetEventMenu(this._ui_SelectMenu);
    }

    //낙인을 받는 유닛을 고르는 함수
    public void OnSelectStigmaTargetUnit()
    {
        GameManager.UI.CloseAllPopup();
        this._ui_SelectMenu.SetActive(false);
        UI_MyDeck ud = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ud.Init(false, OnSelectStigmatransfertarget, CUR_EVENT.RECEIVE_STIGMA);
        ud.SetEventMenu(this._ui_SelectMenu);
    }

    // 낙인을 주는 유닛을 고르는 함수,낙인 이동을 눌렀을 경우에
    public void OnStigmaGiveUnitButtonClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        this._ui_SelectMenu.SetActive(false);
        UI_MyDeck ud = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ud.Init(false, OnSelectStigmatransfergiver, CUR_EVENT.GIVE_STIGMA);
        ud.SetEventMenu(this._ui_SelectMenu);
    }

    // 낙인소 나가기 
    public void OnQuitClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/BackButtonClickSFX");
        StartCoroutine(QuitScene());

    }

    // 낙인 이동 실행시킬 함수
    public void StigmaTransfer()
    {
        GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(null, "Transfer Stigma Get", _givestigmatizeUnit.GetStigma());
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

        if (_stigmatizeUnit.GetStigmaCount()< _stigmatizeUnit._maxStigmaCount)
        {
            Debug.Log("유닛이 스티그마를 더 받을 수 잇는 상태입니다.");
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(_stigmatizeUnit, "Select Stigma", null, 3, null, this);
        }
        else
        {
            Debug.Log("유닛 스티그마 더 받을 수 없으니 하나를 선택하여 지워야 합니다.");
            IsStigmaFull();
        }
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
        ui.Init(null, CUR_EVENT.COMPLETE_STIGMA,OnQuitClick);
    }

    public void OnSelectStigmatransfertarget(DeckUnit unit)
    {
        _stigmatizeUnit = unit;
        if (_stigmatizeUnit.GetStigmaCount() <_stigmatizeUnit._maxStigmaCount)
        {
            Debug.Log("유닛이 스티그마를 더 받을 수 잇는 상태입니다.");
            SetUnitStigma(_giveStigma);

        }
        else
        {
            Debug.Log("유닛 스티그마 더 받을 수 없으니 하나를 선택하여 지워야 합니다.");
            IsStigmaFull();
        }
    }

    public void OnSelectStigmatransfergiver(DeckUnit unit)
    {
        _givestigmatizeUnit = unit;
        GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(null, "Transfer Stigma Give", _givestigmatizeUnit.GetStigma(true),0,null,this);
    }

    public void OnStigmaSelected(Stigma stigma)
    {
        if (_isStigmaFull)//스티그마 예외처리
        {
            Debug.Log("스티그마 꽉차있서요");
            _isStigmaFull = false;
            _stigmatizeUnit.DeleteStigma(stigma);
            GameManager.UI.CloseAllPopup();
            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_stigmatizeUnit);

            if (_givestigmatizeUnit == null)
                unitInfo.Init(OnSelectStigmatization, CUR_EVENT.STIGMA_EXCEPTION);
            else
                unitInfo.Init(OnSelectStigmatransfertarget, CUR_EVENT.STIGMA_EXCEPTION);
            return;
        }
        if (_givestigmatizeUnit == null) 
        {//낙인 부여일때
            SetUnitStigma(stigma);
            GameManager.Data.GameData.NpcQuest.StigmaQuest++;
        }
        else//낙인 이동일때
        {
            _giveStigma = stigma;
            if(!_isNPCFall)
                GameManager.Data.RemoveDeckUnit(_givestigmatizeUnit);
            else
                _givestigmatizeUnit.DeleteStigma(stigma);

            GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
            OnSelectStigmaTargetUnit();
        }
    }
    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        
        if (GameManager.Data.GameData.IsVisitStigma == false)
        {
            GameManager.Data.GameData.IsVisitStigma = true;
        }
        

        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        int questLevel = (int)(GameManager.Data.GameData.NpcQuest.StigmaQuest / 12.5f);
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitStigma() == false && questLevel < 4)
        {
            GameManager.OutGameData.SetVisitStigma(true);
            Debug.Log("GameManager.OutGameData.getVisitStigma() : " + GameManager.OutGameData.GetVisitStigma());
            quitScript.Init(GameManager.Data.ScriptData["낙인소_퇴장_최초"], false);
        }
        else
        {
            quitScript.Init(GameManager.Data.ScriptData[$"낙인소_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
        }
        yield return StartCoroutine(quitScript.PrintScript());
        GameManager.Data.Map.ClearTileID.Add(GameManager.Data.Map.CurrentTileID);
        GameManager.SaveManager.SaveGame();
        SceneChanger.SceneChange("StageSelectScene");
    }

    private void OnConversationEnded()
    {
        _ui_SelectMenu.SetActive(true);
    }
}