using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSceneController : MonoBehaviour
{
    private DeckUnit _unit;

    [SerializeField] private Image _upgradeunitImage; // 강화 대상 유닛

    [SerializeField] private Image _releaseunitImage; // 교화 해소 유닛

    [SerializeField] private Button _forbiddenButton; // 접근 금지 버튼

    void Start()
    {
        Init();
    }

    private void Init()
    {
        List<Script> scripts = new List<Script>();

        if(GameManager.Data.GameData.isVisitUpgrade == false)
            scripts = GameManager.Data.ScriptData["강화소_입장_최초"];
        else
            scripts = GameManager.Data.ScriptData["강화소_입장"];

        GameManager.UI.ShowPopup<UI_Conversation>().Init(scripts);
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

    // 업그레이드 할 유닛을 고릅니다.
    public void OnUpgradeUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelectUpgrade);
    }

    // 교화를 풀 유닛을 고릅니다.
    public void OnReleaseUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelectRelease);
    }

    // 유닛 선택 후 업그레이드 완료 버튼
    public void OnUpgradeButtonClick()
    {
        if (_unit != null)
            GameManager.UI.ShowPopup<UI_UpgradeSelectButton>().init(this);
        //GameManager.Sound.Play("UI/ButtonSFX/ButtonClickSFX");
    }

    // 유닛 선택 후 교화 풀기 버튼
    public void OnReleaseButtonClick()
    {
        if(_unit.DeckUnitChangedStat.FallCurrentCount > 0)
        {
            _unit.DeckUnitChangedStat.FallCurrentCount -= 1;
        }

        StartCoroutine(QuitScene());

    }

    public  void OnSelectUpgrade(DeckUnit unit)
    {
        _unit = unit;
        _upgradeunitImage.sprite = unit.Data.Image;
        _upgradeunitImage.color = Color.white;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }

    public void OnSelectRelease(DeckUnit unit)
    {
        _unit = unit;
        _releaseunitImage.sprite = unit.Data.Image;
        _releaseunitImage.color = Color.white;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }


    

    public void OnUpgradeSelect(int select) 
    {
        GameManager.UI.ClosePopup();
        UI_Conversation script = GameManager.UI.ShowPopup<UI_Conversation>();

        if (select == 1)
        {
            _unit.DeckUnitUpgradeStat.ATK += 5;
            script.Init(GameManager.Data.ScriptData["강화소_공격력"], false);
        }
        else if (select == 2) 
        {
            _unit.DeckUnitUpgradeStat.MaxHP += 15;
            script.Init(GameManager.Data.ScriptData["강화소_체력"], false);
        }
        else if (select == 3)
        {
            _unit.DeckUnitUpgradeStat.SPD += 25;
            script.Init(GameManager.Data.ScriptData["강화소_속도"], false);
        }
        else if (select == 4)
        {
            _unit.DeckUnitUpgradeStat.ManaCost -= 10;
            script.Init(GameManager.Data.ScriptData["강화소_코스트"], false);
        }
        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        // OnQuitClick();
        StartCoroutine(QuitScene(script));
    }

    public void OnQuitClick()
    {
        StartCoroutine(QuitScene());

        if (GameManager.Data.GameData.isVisitUpgrade == false)
        {
            GameManager.Data.GameData.isVisitUpgrade = true;
        }
    }

    private IEnumerator QuitScene(UI_Conversation eventScript = null)
    {
        if (GameManager.Data.GameData.isVisitStigma == false)
        {
            GameManager.Data.GameData.isVisitStigma = true;
        }

        if (eventScript != null)
            yield return StartCoroutine(eventScript.PrintScript());

        UI_Conversation quitScript = GameManager.UI.ShowPopup<UI_Conversation>();

        if (GameManager.Data.GameData.isVisitUpgrade == false)
        {
            GameManager.Data.GameData.isVisitUpgrade = true;
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장_최초"], false);
        }
        else
            quitScript.Init(GameManager.Data.ScriptData["강화소_퇴장"], false);

        yield return StartCoroutine(quitScript.PrintScript());
        GameManager.SaveManager.SaveGame();
        SceneChanger.SceneChange("StageSelectScene");
    }
}