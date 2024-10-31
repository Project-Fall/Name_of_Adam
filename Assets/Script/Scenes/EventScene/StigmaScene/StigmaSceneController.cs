using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StigmaSceneController : MonoBehaviour, StigmaInterface
{
    [SerializeField] private GameObject _normalBackground;
    [SerializeField] private GameObject _corruptBackground;
    [SerializeField] private List<GameObject> _fogImageList;

    [SerializeField] private GameObject _stigmataTransferButton = null;
    [SerializeField] private GameObject _disabledStigmataTransferButton;
    [SerializeField] private GameObject _selectMenuUI;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private TextMeshProUGUI _stigmaBestowalButtonText;

    private UI_Conversation _uiConversation;

    private readonly int[] enterDialogNums = { 3, 2, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    private List<Script> _scripts = new();

    private DeckUnit _stigmataTransferGiveUnit;
    private DeckUnit _stigmataBestowalUnit;
    private Stigma _transferStigmata = null;

    private List<Stigma> _stigmataList = new();
    private Stigma _preSelectedStigmata;
    private bool _isStigmataPreSet = false;

    private bool _isStigmataFull = false;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        //선택지 안 뜨게
        bool canStigmataTransfer = false;

        List<DeckUnit> deckUnit = GameManager.Data.GetDeck();
        foreach (DeckUnit unit in deckUnit)
        {
            if (canStigmataTransfer)
                break;
            
            if (unit.GetStigma(true).Count != 0)
            {
                foreach (DeckUnit otherUnit in deckUnit)
                {
                    if (otherUnit != unit && otherUnit.Data.Rarity != Rarity.Boss)
                    {
                        canStigmataTransfer = true;
                        break;
                    }
                }
            }
        }

        if (!GameManager.OutGameData.IsUnlockedItem(11))
        {
            _stigmataTransferButton.SetActive(false);
        }
        else
        {
            _stigmataTransferButton.SetActive(canStigmataTransfer);
            _disabledStigmataTransferButton.SetActive(!canStigmataTransfer);
        }

        Debug.Log($"횟수: {GameManager.OutGameData.Data.StigmataCorruptValue}");

        int questLevel = Mathf.Min((int)(GameManager.OutGameData.Data.StigmataCorruptValue / 10f), 4);
        questLevel = 0;
        if (questLevel == 4 && !GameManager.OutGameData.Data.IsStigmataCorrupt && GameManager.OutGameData.Data.YohrnClear)
        {
            GameManager.OutGameData.Data.IsStigmataCorrupt = true;
            /*
            DeckUnit unit = new()
            {
                Data = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/믿음을_저버린_자"),
                IsMainDeck = false,
                PrivateKey = "Origin_Betrayer_Of_Faith",
                HallUnitID = -1
            };

            GameManager.OutGameData.AddHallUnit(unit);
            GameManager.Data.AddDeckUnit(unit);
            GameManager.Data.GameData.FallenUnits.Add(unit);
            */
        }

        if (GameManager.OutGameData.Data.IsStigmataCorrupt)
        {
            _normalBackground.SetActive(false);
            _corruptBackground.SetActive(true);
            _stigmaBestowalButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Stigmata Bestowal_Corrupt"));
        }
        else
        {
            _normalBackground.SetActive(true);
            _corruptBackground.SetActive(false);
            _stigmaBestowalButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Stigmata Bestowal"));
        }

        if (!GameManager.OutGameData.Data.IsVisitStigmata)
        {
            _scripts = GameManager.Data.ScriptData["낙인소_입장_최초"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["낙인소_선택_0"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["낙인소_선택_0"][0].name));
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"낙인소_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"낙인소_선택_{25 * questLevel}"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"낙인소_선택_{25 * questLevel}"][0].name));
        }

        for (int i = 0; i < 3; i++)
        {
            _fogImageList[i].gameObject.SetActive(questLevel > i);
        }

        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
        _uiConversation = FindObjectOfType<UI_Conversation>();
        _uiConversation.ConversationEnded += OnConversationEnded;
    }

    //성흔 부여 버튼 클릭
    public void OnStigmataBestowalButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _selectMenuUI.SetActive(false);

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectStigmataBestowalUnit, CurrentEvent.Stigmata_Select, _selectMenuUI);
    }

    //성흔 이동 버튼 클릭
    public void OnStigmataTransferButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _selectMenuUI.SetActive(false);

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectStigmataTransferGiver, CurrentEvent.Stigmata_Give, _selectMenuUI);
    }

    //성흔 부여 대상 선택
    public void OnSelectStigmataBestowalUnit(DeckUnit unit)
    {
        _stigmataBestowalUnit = unit;

        if (_stigmataBestowalUnit.GetStigmaCount() < _stigmataBestowalUnit.MaxStigmaCount || _isStigmataPreSet)
        {
            ResetStigmataList(unit);

            UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
            popup.Init(_stigmataBestowalUnit, false, _stigmataList);
            popup.EventInit(this, CurrentEvent.Stigmata_Select);
        }
        else
        {
            UnitStigmataFull();
        }

        //선 저장
        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
    }

    //줄 성흔을 고르는 함수
    public void OnSelectStigmataTransferGiver(DeckUnit unit)
    {
        _stigmataTransferGiveUnit = unit;

        UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
        popup.Init(null, false, _stigmataTransferGiveUnit.GetStigma(true));
        popup.EventInit(this, CurrentEvent.Stigmata_Give);
    }

    //성흔을 받는 유닛을 고르는 함수
    public void SelectStigmataTransferReceiver()
    {
        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        GameManager.UI.CloseAllPopup();
        _selectMenuUI.SetActive(false);

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectStigmataTransferReceiver, CurrentEvent.Stigmata_Receive, _selectMenuUI);
    }

    public void OnSelectStigmataTransferReceiver(DeckUnit unit)
    {
        if (unit.CheckStigma(_transferStigmata.StigmaEnum, _transferStigmata.Tier))
        {
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("AlreadyExistStigmataInfo", "AlreadyExistStigmataTooltip", () => { GameManager.UI.ClosePopup(); });
            return;
        }

        _stigmataBestowalUnit = unit;

        if (_stigmataBestowalUnit.GetStigmaCount() < _stigmataBestowalUnit.MaxStigmaCount)
        {
            BestowalStigmata(_transferStigmata);
            GameManager.Data.Map.SetCurrentTileClear();
            GameManager.SaveManager.SaveGame();
        }
        else
        {
            UnitStigmataFull();
        }
    }

    public void UnitStigmataFull()
    {
        _isStigmataFull = true;

        UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
        popup.Init(null, true, _stigmataBestowalUnit.GetStigma(true));
        popup.EventInit(this, CurrentEvent.Stigmata_Full_Exception);
    }

    public void OnStigmataSelected(Stigma stigmata)
    {
        //UI에서 선택된 성흔
        if (_isStigmataFull)
        {
            //성흔 예외 처리 시 실행
            _isStigmataFull = false;
            _stigmataBestowalUnit.DeleteStigma(stigmata);

            GameManager.UI.CloseAllPopup();
            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_stigmataBestowalUnit);

            if (_stigmataTransferGiveUnit == null)
            {
                //성흔 부여에서 예외 처리 시
                ResetStigmataList(_stigmataBestowalUnit);   
                unitInfo.Init(OnSelectStigmataBestowalUnit, CurrentEvent.Stigmata_Full_Exception);
            }
            else
            {
                //성흔 이동에서 예외 처리 시
                unitInfo.Init(OnSelectStigmataTransferReceiver, CurrentEvent.Stigmata_Full_Exception);
            }
        }
        else if (_stigmataTransferGiveUnit == null)
        {
            //성흔 부여일때
            BestowalStigmata(stigmata);
            GameManager.OutGameData.Data.StigmataCorruptValue++;
        }
        else
        {
            //성흔 이동일때
            _transferStigmata = stigmata;
            GameManager.Data.RemoveDeckUnit(_stigmataTransferGiveUnit);
            SelectStigmataTransferReceiver();
        }
    }

    private void BestowalStigmata(Stigma stigmata)
    {
        if (_isStigmataPreSet)
        {
            _stigmataBestowalUnit.DeleteStigma(_preSelectedStigmata);
            _isStigmataPreSet = false;
        }
        _stigmataBestowalUnit.AddStigma(stigmata);

        GameManager.Sound.Play("UI/UISFX/UISuccessSFX");
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();

        UI_UnitInfo ui = GameManager.UI.ShowPopup<UI_UnitInfo>();
        ui.SetUnit(_stigmataBestowalUnit);
        ui.Init(null, CurrentEvent.Complate_Stigmata, OnQuitClick);
    }

    public List<Stigma> ResetStigmataList(DeckUnit stigmataTargetUnit)
    {
        _stigmataList.Clear();
        if (GameManager.OutGameData.Data.IsStigmataCorrupt)
            _stigmataList = GameManager.Data.StigmaController.GetRandomStigmaList(stigmataTargetUnit, 4);
        else
            _stigmataList = GameManager.Data.StigmaController.GetRandomStigmaList(stigmataTargetUnit, 3);

        //선 적용된 성흔 리셋
        if (_isStigmataPreSet)
        {
            _stigmataBestowalUnit.DeleteStigma(_preSelectedStigmata);
        }

        _preSelectedStigmata = _stigmataList[Random.Range(0, _stigmataList.Count)];

        if (_stigmataBestowalUnit.GetStigmaCount() < _stigmataBestowalUnit.MaxStigmaCount)
        {
            _stigmataBestowalUnit.AddStigma(_preSelectedStigmata);
            GameManager.SaveManager.SaveGame();

            _isStigmataPreSet = true;
        }

        return _stigmataList;
    }

    //나가기 
    public void OnQuitClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        StartCoroutine(QuitScene());
    }

    private void OnConversationEnded()
    {
        _selectMenuUI.SetActive(true);
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        if (!GameManager.OutGameData.Data.IsVisitStigmata)
        {
            GameManager.OutGameData.Data.IsVisitStigmata = true;
            quitScript.Init(GameManager.Data.ScriptData["낙인소_퇴장_최초"], false);
        }
        else
        {
            int questLevel = (int)(GameManager.OutGameData.Data.StigmataCorruptValue / 7.5f);
            if (questLevel > 4) questLevel = 4;
            quitScript.Init(GameManager.Data.ScriptData[$"낙인소_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
        }
        yield return StartCoroutine(quitScript.PrintScript());
        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
        GameManager.OutGameData.SaveData();
        SceneChanger.SceneChange("StageSelectScene");
    }
}