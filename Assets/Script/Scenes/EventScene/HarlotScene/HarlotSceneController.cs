using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class HarlotSceneController : MonoBehaviour, StigmaInterface
{
    private readonly int[] enterDialogNums = { 3, 3, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    [SerializeField] private GameObject _normalBackground;
    [SerializeField] private GameObject _corruptBackground;
    [SerializeField] private List<GameObject> _fogImageList;

    [SerializeField] private GameObject _stigmataBestowalButton;
    [SerializeField] private GameObject _disabledStigmataBestowalButton;
    [SerializeField] private GameObject _apostleCreationButton;
    [SerializeField] private GameObject _disabledApostleCreationButton;
    [SerializeField] private GameObject _selectMenuUI;

    [SerializeField] private List<DeckUnit> _originUnits = null;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private TextMeshProUGUI _revertUnitDarkEssenceText;
    [SerializeField] private TextMeshProUGUI _apostleCreationDarkEssenceText;
    [SerializeField] private TextMeshProUGUI _disabledApostleCreationDarkEssenceText;
    [SerializeField] private TextMeshProUGUI _stigmataBestowalDarkEssenceText;
    [SerializeField] private TextMeshProUGUI _disabledStigmataBestowalDarkEssenceText;

    [SerializeField] private TextMeshProUGUI _revertUnitButtonText;
    [SerializeField] private TextMeshProUGUI _stigmataBestowalButtonText_enable;
    [SerializeField] private TextMeshProUGUI _stigmataBestowalButtonText_disable;

    private UI_Conversation _conversationUI;

    private List<Script> _scripts = new();

    private DeckUnit _stigmataBestowalUnit;
    private List<Stigma> _stigmataList = new();
    private List<DeckUnit> _revertUnits = new();

    private Stigma _preSelectedStigmata;
    private bool _isStigmataPreSet = false;

    private bool _isStigmataFull = false;
    private bool _isDarkEssenceUsed = false;

    private int _revertUnitDarkEssence;
    private int _stigmataBestowalDarkEssence;
    private int _apostleCreationDarkEssence;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log($"횟수: {GameManager.OutGameData.Data.SacrificeCorruptValue}");

        int questLevel = Mathf.Min((int)(GameManager.OutGameData.Data.SacrificeCorruptValue / 20f), 4);
        questLevel = 0;
        if (questLevel == 4 && GameManager.OutGameData.Data.SaviorClear && !GameManager.OutGameData.Data.IsSacrificeCorrupt)
        {
            GameManager.OutGameData.Data.IsSacrificeCorrupt = true;
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

        if (GameManager.OutGameData.Data.IsSacrificeCorrupt)
        {
            _normalBackground.SetActive(false);
            _corruptBackground.SetActive(true);
            _revertUnitDarkEssence = 2;
            _stigmataBestowalDarkEssence = 8;
            _revertUnitButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Revert Unit_Corrupt"));
            _stigmataBestowalButtonText_enable.SetText(GameManager.Locale.GetLocalizedEventScene("Corruption Stigmata Bestowal_Corrupt"));
            _stigmataBestowalButtonText_disable.SetText(GameManager.Locale.GetLocalizedEventScene("Corruption Stigmata Bestowal_Corrupt"));
        }
        else
        {
            _normalBackground.SetActive(true);
            _corruptBackground.SetActive(false);
            _revertUnitDarkEssence = 1;
            _stigmataBestowalDarkEssence = 12;
            _revertUnitButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Revert Unit"));
            _stigmataBestowalButtonText_enable.SetText(GameManager.Locale.GetLocalizedEventScene("Corruption Stigmata Bestowal"));
            _stigmataBestowalButtonText_disable.SetText(GameManager.Locale.GetLocalizedEventScene("Corruption Stigmata Bestowal"));
        }

        //_apostleCreationDarkEssence = (_isNPCFall) ? 10 : 15;
        _apostleCreationDarkEssence = 15;

        if (!GameManager.OutGameData.Data.IsVisitSacrifice)
        {
            _scripts = GameManager.Data.ScriptData["탕녀_입장_최초"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["탕녀_선택_0"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["탕녀_선택_0"][0].name));
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"탕녀_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"탕녀_선택_{25 * questLevel}"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"탕녀_선택_{25 * questLevel}"][0].name));
        }

        for (int i = 0; i < 3; i++)
        {
            _fogImageList[i].gameObject.SetActive(questLevel > i);
        }

        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
        _conversationUI = FindObjectOfType<UI_Conversation>();
        _conversationUI.ConversationEnded += OnConversationEnded;

        int current_DarkEssense = GameManager.Data.GameData.DarkEssence;

        if (!GameManager.OutGameData.IsUnlockedItem(SanctumUnlock.ApostleCreation))
        {
            _apostleCreationButton.SetActive(false);
        }
        else if (current_DarkEssense < _apostleCreationDarkEssence) 
        {
            _disabledApostleCreationButton.SetActive(true);
            _apostleCreationButton.SetActive(false);
        }

        if (current_DarkEssense < _stigmataBestowalDarkEssence)
        {
            _disabledStigmataBestowalButton.SetActive(true);
            _stigmataBestowalButton.SetActive(false);
        }

        SetMenuText();
    }

    //사도 연성 버튼 클릭
    public void OnApostleCreationButtonClick()
    {
        string corruption = (GameManager.OutGameData.Data.IsSacrificeCorrupt) ? "Corrupt" : "Normal";

        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        GameManager.UI.ShowPopup<UI_SystemSelect>().Init($"ApostleCreation_{corruption}", YesApostleCreationButtonClick);
    }

    private void YesApostleCreationButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        _selectMenuUI.SetActive(false);

        DeckUnit originalUnit = new();
        originalUnit.Data = _originUnits[Random.Range(0, 3)].Data;
        originalUnit.IsMainDeck = false;
        originalUnit.HallUnitID = -1;

        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(originalUnit);
        unitInfo.Init(OnApostleSelect, CurrentEvent.Complate_Apostle, OnQuitClick);
    }

    public void OnApostleSelect(DeckUnit unit)
    {
        GameManager.Data.AddDeckUnit(unit);
        GameManager.Data.DarkEssenseChage(-_apostleCreationDarkEssence);
        GameManager.Data.GameData.FallenUnits.Add(unit);
    }

    //유닛을 검은 정수로 환원하는 버튼
    public void OnRevertUnitButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        _revertUnits.Clear();

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectRevertUnit, CurrentEvent.Revert_Unit_Select, _selectMenuUI, RevertUnitQuitClick);
    }

    public void OnSelectRevertUnit(DeckUnit unit)
    {
        if (!_revertUnits.Contains(unit))
        {
            _revertUnits.Add(unit);
        }
        else
        {
            _revertUnits.Remove(unit);
        }

        GameManager.UI.ClosePopup();
    }

    public void RevertUnitQuitClick()
    {
        if (_revertUnits.Count == GameManager.Data.GetDeck().Count)
        {
            GameManager.UI.ShowPopup<UI_SystemInfo>().Init("RevertAll", "");
            return;
        }
        else if (_revertUnits.Count != 0)
        {
            GameManager.Data.DarkEssenseChage(_revertUnitDarkEssence * _revertUnits.Count);
            foreach (DeckUnit delunit in _revertUnits)
                GameManager.Data.RemoveDeckUnit(delunit);
        }

        GameManager.UI.ClosePopup();
        OnQuitClick();
    }

    //타락 성흔 부여 버튼 클릭
    public void OnStigmataBestowalButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectStigmataBestowalUnit, CurrentEvent.Corrupt_Stigmata_Select, _selectMenuUI);
    }

    public void UnitStigmataFull()
    {
        UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
        popup.Init(null, true, _stigmataBestowalUnit.GetStigma(true));
        popup.EventInit(this, CurrentEvent.Stigmata_Full_Exception);

        _isStigmataFull = true;
    }

    public void OnSelectStigmataBestowalUnit(DeckUnit unit)
    {
        if (unit.CheckHaveAnyCorruptStigmata() && !_isStigmataPreSet)
        {
            GameManager.UI.ShowPopup<UI_SystemSelect>().Init("CorfirmAlreadyHaveCorruptStigmata", () =>
            {
                GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

                List<Stigma> stigmataList = new();
                _stigmataBestowalUnit = unit;

                foreach (Stigma stigmata in unit.GetStigma())
                {
                    if (stigmata.Tier == StigmaTier.Harlot)
                        stigmataList.Add(stigmata);
                }

                UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
                popup.Init(null, true, stigmataList);
                popup.EventInit(this, CurrentEvent.Stigmata_Full_Exception);

                _isStigmataFull = true;
            }, () => {
                GameManager.UI.CloseAllPopup();
                OnStigmataBestowalButtonClick();
                return;
            });
        }
        else if (unit.GetStigmaCount() < unit.MaxStigmaCount || _isStigmataPreSet)
        {
            _stigmataBestowalUnit = unit;

            ResetStigmataList(unit);

            if (_isDarkEssenceUsed == false)
            {
                _isDarkEssenceUsed = true;
                GameManager.Data.DarkEssenseChage(-_stigmataBestowalDarkEssence);
            }

            UI_StigmaSelectButtonPopup popup = GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>();
            popup.Init(_stigmataBestowalUnit, false, _stigmataList);
            popup.EventInit(this, CurrentEvent.Stigmata_Select);
        }
        else
        {
            _stigmataBestowalUnit = unit;

            UnitStigmataFull();
        }

        //선 저장
        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
    }

    private void BestowalStigmata(Stigma stigma)
    {
        if (_isStigmataPreSet)
        {
            _stigmataBestowalUnit.DeleteStigma(_preSelectedStigmata);
            _isStigmataPreSet = false;
        }
        _stigmataBestowalUnit.AddStigma(stigma);

        GameManager.Sound.Play("UI/UISFX/UISuccessSFX");
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(_stigmataBestowalUnit);
        unitInfo.Init(null, CurrentEvent.Complate_Stigmata, OnQuitClick);
    }

    public void OnStigmataSelected(Stigma stigmata)
    {
        if (_isStigmataFull)
        {
            _isStigmataFull = false;
            _stigmataBestowalUnit.DeleteStigma(stigmata);

            GameManager.UI.CloseAllPopup();
            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_stigmataBestowalUnit);

            ResetStigmataList(_stigmataBestowalUnit);
            unitInfo.Init(OnSelectStigmataBestowalUnit, CurrentEvent.Stigmata_Full_Exception);
        }
        else
        {
            BestowalStigmata(stigmata);
        }
    }

    //대화하기 버튼을 클릭했을 경우
    public void OnConversationButtonClick()
    {
        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
    }

    public List<Stigma> ResetStigmataList(DeckUnit stigmataTargetUnit)
    {
        _stigmataList.Clear();
        _stigmataList = GameManager.Data.StigmaController.GetRandomHarlotStigmaList(stigmataTargetUnit, 2);

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

    public void OnQuitClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        StartCoroutine(QuitScene());
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        if (!GameManager.OutGameData.Data.IsVisitSacrifice)
        {
            GameManager.OutGameData.Data.IsVisitSacrifice = true;
            quitScript.Init(GameManager.Data.ScriptData["탕녀_퇴장_최초"], false);
        }
        else 
        {
            int questLevel = (int)(GameManager.OutGameData.Data.SacrificeCorruptValue / 12.5f);
            if (questLevel > 4) questLevel = 4;
            quitScript.Init(GameManager.Data.ScriptData[$"탕녀_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
        }
        
        yield return StartCoroutine(quitScript.PrintScript());
        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
        GameManager.OutGameData.SaveData();
        SceneChanger.SceneChange("StageSelectScene");
    }

    private void SetMenuText()
    {
        _revertUnitDarkEssenceText.text = _revertUnitDarkEssence.ToString();
        _stigmataBestowalDarkEssenceText.text = _stigmataBestowalDarkEssence.ToString();
        _disabledStigmataBestowalDarkEssenceText.text = _stigmataBestowalDarkEssence.ToString();

        _apostleCreationDarkEssenceText.text = _apostleCreationDarkEssence.ToString();
        _disabledApostleCreationDarkEssenceText.text = _apostleCreationDarkEssence.ToString();
    }

    private void OnConversationEnded()
    {
        _selectMenuUI.SetActive(true);
    }
}
