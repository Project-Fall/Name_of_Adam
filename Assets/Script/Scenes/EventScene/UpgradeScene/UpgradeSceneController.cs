using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSceneController : MonoBehaviour
{
    private readonly int[] enterDialogNums = { 3, 3, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    [SerializeField] private GameObject _normalBackground;
    [SerializeField] private GameObject _corruptBackground;
    [SerializeField] private List<GameObject> _fogImageList;

    [SerializeField] private GameObject _healFaithButton;
    [SerializeField] private GameObject _selectMenuUI;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    [SerializeField] private TextMeshProUGUI _upgradeButtonText;

    private List<Script> _scripts = new();
    private UI_Conversation _conversationUI;

    private DeckUnit _selectedUnit;

    private List<Upgrade> _upgradeList = new();
    private Upgrade _preSelectedUpgrade;
    private bool _isUpgradePreSet = false;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (!GameManager.OutGameData.IsUnlockedItem(2))
        {
            _healFaithButton.SetActive(false);
        }

        Debug.Log($"횟수: {GameManager.OutGameData.Data.BaptismCorruptValue}");

        int questLevel = Mathf.Min(GameManager.OutGameData.Data.BaptismCorruptValue / 30, 4);
        questLevel = 0;
        if (questLevel == 4 && GameManager.OutGameData.Data.PhanuelClear && !GameManager.OutGameData.Data.IsBaptismCorrupt)
        {
            GameManager.OutGameData.Data.IsBaptismCorrupt = true;
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
        }

        if (GameManager.OutGameData.Data.IsBaptismCorrupt)
        {
            _normalBackground.SetActive(false);
            _corruptBackground.SetActive(true);
            _upgradeButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Upgrade_Corrupt"));

        }
        else
        {
            _normalBackground.SetActive(true);
            _corruptBackground.SetActive(false);
            _upgradeButtonText.SetText(GameManager.Locale.GetLocalizedEventScene("Upgrade"));
        }

        if (!GameManager.OutGameData.Data.IsVisitBaptism)
        {
            _scripts = GameManager.Data.ScriptData["강화소_입장_최초"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["강화소_선택_0"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["강화소_선택_0"][0].name));
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"강화소_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            _descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"강화소_선택_{25 * questLevel}"][0].script));
            _nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"강화소_선택_{25 * questLevel}"][0].name));
        }

        for (int i = 0; i < 3; i++)
        {
            _fogImageList[i].gameObject.SetActive(questLevel > i);
        }

        _conversationUI = GameManager.UI.ShowPopup<UI_Conversation>();
        _conversationUI.Init(_scripts);
        _conversationUI.ConversationEnded += OnConversationEnded;
    }

    //강화 버튼 클릭, 업그레이드 할 유닛을 고릅니다.
    public void OnUpgradeUnitButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _selectMenuUI.SetActive(false);

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectUpgradeUnit, CurrentEvent.Upgrade_Select, _selectMenuUI);
    }

    // 신앙 회복 버튼 클릭, 신앙을 회복할 유닛을 고릅니다.
    public void OnHealFaithButtonClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _selectMenuUI.SetActive(false);

        UI_MyDeck myDeck = GameManager.UI.ShowPopup<UI_MyDeck>();
        myDeck.Init();
        myDeck.EventInit(OnSelectHealFaithUnit, CurrentEvent.Heal_Faith_Select, _selectMenuUI);
    }

    //대화하기 버튼
    public void OnConversationButtonClick()
    {
        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
    }

    public List<Upgrade> ResetUpgrade()
    {
        _upgradeList.Clear();

        int createUpgradeCount = (GameManager.OutGameData.Data.IsBaptismCorrupt) ? 4 : 3;

        while (_upgradeList.Count < createUpgradeCount)
        {
            Upgrade upgrade = GameManager.Data.UpgradeController.GetRandomUpgrade(_selectedUnit);

            if (!_upgradeList.Contains(upgrade))
            {
                _upgradeList.Add(upgrade);
            }
        }

        //선 적용된 강화 리셋
        if (_isUpgradePreSet)
        {
            _selectedUnit.DeckUnitUpgrade.Remove(_preSelectedUpgrade);
        }

        _preSelectedUpgrade = _upgradeList[Random.Range(0, _upgradeList.Count)];

        if ((_selectedUnit.DeckUnitUpgrade.Count == 3 || (_selectedUnit.DeckUnitUpgrade.Count == 2 && !GameManager.OutGameData.IsUnlockedItem(12))) == false)
        {
            _selectedUnit.DeckUnitUpgrade.Add(_preSelectedUpgrade);
            GameManager.SaveManager.SaveGame();

            _isUpgradePreSet = true;
        }

        return _upgradeList;
    }

    public void OnSelectUpgradeUnit(DeckUnit unit)
    {
        //강화할 유닛을 선택함
        _selectedUnit = unit;

        if (_selectedUnit.DeckUnitUpgrade.Count == 3 || (_selectedUnit.DeckUnitUpgrade.Count == 2 && !GameManager.OutGameData.IsUnlockedItem(12)))
        {
            GameManager.UI.ShowPopup<UI_UpgradeSelectButtonPopup>().Init(this, _selectedUnit.DeckUnitUpgrade, true);
        }
        else
        {
            ResetUpgrade();
            GameManager.UI.ShowPopup<UI_UpgradeSelectButtonPopup>().Init(this, _upgradeList, false);
        }

        //선 저장
        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
    }

    public void OnSelectHealFaithUnit(DeckUnit unit)
    {
        _selectedUnit = unit;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();

        int releaseVal = 2;
        if (_selectedUnit.DeckUnitStat.FallCurrentCount - releaseVal > 0)
            _selectedUnit.DeckUnitUpgradeStat.FallCurrentCount -= releaseVal;
        else
            _selectedUnit.DeckUnitUpgradeStat.FallCurrentCount = -_selectedUnit.Data.RawStat.FallCurrentCount;

        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(_selectedUnit);
        unitInfo.Init(null, CurrentEvent.Complete_Heal_Faith, OnQuitClick);
    }

    public void OnUpgradeSelect(int select)
    {
        GameManager.UI.CloseAllPopup();

        _selectedUnit.DeckUnitUpgrade.Remove(_preSelectedUpgrade);
        _selectedUnit.DeckUnitUpgrade.Add(_upgradeList[select]);
        GameManager.Sound.Play("UI/UISFX/UISuccessSFX");

        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(_selectedUnit);
        unitInfo.Init(null, CurrentEvent.Complete_Upgrade, OnQuitClick);
    }

    public void OnDestroyUpgradeSelect(int select)
    {
        GameManager.UI.CloseAllPopup();

        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(_selectedUnit);
        unitInfo.Init(null, CurrentEvent.Upgrade_Full_Exception);

        _selectedUnit.DeckUnitUpgrade.Remove(_selectedUnit.DeckUnitUpgrade[select]);
        ResetUpgrade();
        GameManager.UI.ShowPopup<UI_UpgradeSelectButtonPopup>().Init(this, _upgradeList, false);
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

        if (!GameManager.OutGameData.Data.IsVisitBaptism)
        {
            GameManager.OutGameData.Data.IsVisitBaptism = true;
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장_최초"], false);
        }
        else
        {
            int questLevel = GameManager.OutGameData.Data.BaptismCorruptValue / 75;
            if (questLevel > 4) questLevel = 4;
            quitScript.Init(GameManager.Data.ScriptData[$"강화소_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
        }

        yield return StartCoroutine(quitScript.PrintScript());

        GameManager.Data.Map.SetCurrentTileClear();
        GameManager.SaveManager.SaveGame();
        GameManager.OutGameData.SaveData();
        SceneChanger.SceneChange("StageSelectScene");
    }

    private void OnConversationEnded()
    {
        _selectMenuUI.SetActive(true);
    }
}