using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSceneController : MonoBehaviour
{
    private readonly int[] enterDialogNums = { 3, 3, 3, 3, 3 };
    private readonly int[] exitDialogNums = { 1, 1, 1, 1, 1 };

    private DeckUnit _unit;

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject fall_background;
    [SerializeField] private Image foogyImg;

    [SerializeField] private Button _forbiddenButton; // 접근 금지 버튼
    [SerializeField] private GameObject _restoreFall_Btn;
    [SerializeField] private GameObject _ui_SelectMenu;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private List<Script> _scripts;
    private UI_Conversation _conversationUI;
    private List<Upgrade> _upgradeList = new();

    private bool _isUpgradeFull = false;
    private bool _isNPCFall = false;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (!GameManager.OutGameData.IsUnlockedItem(2))
        {
            _restoreFall_Btn.SetActive(false);
        }

        _scripts = new ();

        Debug.Log($"횟수: {GameManager.Data.GameData.NpcQuest.UpgradeQuest}");

        int questLevel = GameManager.Data.GameData.NpcQuest.UpgradeQuest / 25;
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitUpgrade() == false && questLevel != 4)
        {
            _scripts = GameManager.Data.ScriptData["강화소_입장_최초"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData["강화소_선택_0"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData["강화소_선택_0"][0].name));
            //GameManager.OutGameData.setVisitUpgrade(true);
        }
        else
        {
            _scripts = GameManager.Data.ScriptData[$"강화소_입장_{25 * questLevel}_랜덤코드:{Random.Range(0, enterDialogNums[questLevel])}"];
            descriptionText.SetText(GameManager.Locale.GetLocalizedScriptInfo(GameManager.Data.ScriptData[$"강화소_선택_{25 * questLevel}"][0].script));
            nameText.SetText(GameManager.Locale.GetLocalizedScriptName(GameManager.Data.ScriptData[$"강화소_선택_{25 * questLevel}"][0].name));

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

        _conversationUI = GameManager.UI.ShowPopup<UI_Conversation>();
        _conversationUI.Init(_scripts);
        _conversationUI.ConversationEnded += OnConversationEnded;
    }

    // 업그레이드 할 유닛을 고릅니다.
    public void OnUpgradeUnitButtonClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        _ui_SelectMenu.SetActive(false);
        UI_MyDeck ui = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ui.Init(false, OnSelectUpgrade, CUR_EVENT.UPGRADE, null);
        ui.SetEventMenu(_ui_SelectMenu);
    }

    // 교화를 풀 유닛을 고릅니다.
    public void OnReleaseUnitButtonClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        _ui_SelectMenu.SetActive(false);
        UI_MyDeck ui = GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck");
        ui.Init(false, OnSelectRelease, CUR_EVENT.RELEASE);
        ui.SetEventMenu(_ui_SelectMenu);
    }

    //대화하기 버튼
    public void OnConversationButtonClick()
    {
        GameManager.UI.ShowPopup<UI_Conversation>().Init(_scripts);
    }

    public List<Upgrade> ResetUpgrade()
    {
        _upgradeList.Clear();

        while (_upgradeList.Count < 3)
        {
            Upgrade upgrade = GameManager.Data.UpgradeController.GetRandomUpgrade(_unit);

            if (!_upgradeList.Contains(upgrade))
            {
                _upgradeList.Add(upgrade);
            }
        }

        return _upgradeList;
    }

    public void OnSelectUpgrade(DeckUnit unit)
    {
        _unit = unit;

        while (_upgradeList.Count < 3)
        {
            Upgrade upgrade = GameManager.Data.UpgradeController.GetRandomUpgrade(unit);

            if (!_upgradeList.Contains(upgrade))
            {
                _upgradeList.Add(upgrade);
            }
        }

        if (_unit.DeckUnitUpgrade.Count == 3 || (_unit.DeckUnitUpgrade.Count == 2 && !GameManager.OutGameData.IsUnlockedItem(12)))
        {
            GameManager.UI.ShowPopup<UI_UpgradeSelectButton>().Init(this, _unit.DeckUnitUpgrade, "Full Upgrade");
            _isUpgradeFull = true;
        }
        else
        {
            GameManager.UI.ShowPopup<UI_UpgradeSelectButton>().Init(this, _upgradeList, "Select Upgrade");
            _isUpgradeFull = false;
        }
    }

    public void OnSelectRelease(DeckUnit unit)
    {
        _unit = unit;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
        
        int releaseVal = (_isNPCFall) ? 4 : 2;
        if (_unit.DeckUnitStat.FallCurrentCount - releaseVal > 0)
            _unit.DeckUnitUpgradeStat.FallCurrentCount -= releaseVal;
        else
            _unit.DeckUnitUpgradeStat.FallCurrentCount = -_unit.Data.RawStat.FallCurrentCount;

        UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
        unitInfo.SetUnit(_unit);
        unitInfo.Init(null, CUR_EVENT.COMPLETE_RELEASE,OnQuitClick);
    }

    public void OnUpgradeSelect(int select)
    {
        GameManager.UI.CloseAllPopup();

        if (_isUpgradeFull)
        {
            _isUpgradeFull = false;

            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_unit);
            unitInfo.Init(null, CUR_EVENT.UPGRADE_EXCEPTION);

            _unit.DeckUnitUpgrade.Remove(_unit.DeckUnitUpgrade[select]);
            GameManager.UI.ShowPopup<UI_UpgradeSelectButton>().Init(this, _upgradeList, "Select Upgrade");
        }
        else
        {
            _unit.DeckUnitUpgrade.Add(_upgradeList[select]);
            GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");

            UI_UnitInfo unitInfo = GameManager.UI.ShowPopup<UI_UnitInfo>();
            unitInfo.SetUnit(_unit);
            unitInfo.Init(null, CUR_EVENT.COMPLETE_UPGRADE, OnQuitClick);

        }
    }

    public void OnQuitClick()
    {
        GameManager.Sound.Play("UI/ButtonSFX/BackButtonClickSFX");
        StartCoroutine(QuitScene());
        if (GameManager.Data.GameData.IsVisitUpgrade == false)
        {
            GameManager.Data.GameData.IsVisitUpgrade = true;
        }
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (GameManager.Data.GameData.IsVisitUpgrade == false)
        {
            GameManager.Data.GameData.IsVisitUpgrade = true;
        }

        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        int questLevel = GameManager.Data.GameData.NpcQuest.UpgradeQuest / 25;
        if (questLevel > 4) 
            questLevel = 4;

        if (GameManager.OutGameData.GetVisitUpgrade() == false && questLevel < 4)
        {
            GameManager.OutGameData.SetVisitUpgrade(true);
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장_최초"], false);
        }
        else
        {
            quitScript.Init(GameManager.Data.ScriptData[$"강화소_퇴장_{25 * questLevel}_랜덤코드:{Random.Range(0, exitDialogNums[questLevel])}"], false);
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