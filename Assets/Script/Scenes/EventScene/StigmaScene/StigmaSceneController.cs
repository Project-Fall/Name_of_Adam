using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StigmaSceneController : MonoBehaviour
{
    private DeckUnit _givestigmatizeUnit;
    private DeckUnit _stigmatizeUnit;
    [SerializeField] private Image _unitImage;
    [SerializeField] private Image _giveunitImage;
    [SerializeField] private Image _targetunitImage;

    [SerializeField] private Button _forbiddenButton; // 접근 금지 버튼

    void Start()
    {
        Init();
    }

    private void Init()
    {
        List<Script> scripts = new();

        if (GameManager.Data.GameData.isVisitUpgrade == false)
            scripts = GameManager.Data.ScriptData["낙인소_입장_최초"];
        else
            scripts = GameManager.Data.ScriptData["낙인소_입장"];

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

    // 유닛 선택창을 띄우는 함수
    public void OnStigmaUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelectStigmatization);
    }


    public void OnStigmaTargetUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelectStigmatransfertarget);
    }

    // 낙인을 주는 유닛을 고르는 함수
    public void OnStigmaGiveUnitButtonClick()
    {
        GameManager.UI.ShowPopup<UI_MyDeck>("UI_MyDeck").Init(false, OnSelectStigmatransfergiver);
    }

    // 낙인 선택지가 뜨는 함수
    public void OnStigmaButtonClick()
    {
        if (_stigmatizeUnit != null)
        {
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(_stigmatizeUnit, null, 3);
        }
    }

    // 낙인소 나가기 
    public void OnQuitClick()
    {
        StartCoroutine(QuitScene());
    }

    // 낙인 이동 실행시킬 함수
    public void StigmaTransfer()
    {
        if (_stigmatizeUnit != null)
        {
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(_stigmatizeUnit, _givestigmatizeUnit.Stigma);
        }
    }

    public void OnSelectStigmatization(DeckUnit unit)
    {
        _stigmatizeUnit = unit;
        _unitImage.sprite = unit.Data.Image;
        _unitImage.color = Color.white;
        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }

    public void OnSelectStigmatransfertarget(DeckUnit unit)
    {
        _stigmatizeUnit = unit;
        _targetunitImage.sprite = unit.Data.Image;
        _targetunitImage.color = Color.white;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }

    public void OnSelectStigmatransfergiver(DeckUnit unit)
    {
        _givestigmatizeUnit = unit;
        _giveunitImage.sprite = unit.Data.Image;
        _giveunitImage.color = Color.white;

        GameManager.UI.ClosePopup();
        GameManager.UI.ClosePopup();
    }


    public void OnStigmaSelected(Stigma stigma)
    {
        _stigmatizeUnit.AddStigma(stigma);
        GameManager.UI.ClosePopup();
        AddStigmaScript(stigma);
        //StartCoroutine(QuitScene());
        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        //OnQuitClick();
    }

    public void AddStigmaScript(Stigma stigma)
    { 
        UI_Conversation script = GameManager.UI.ShowPopup<UI_Conversation>();
        string scriptKey = "낙인소_" + stigma.GetName();
        script.Init(GameManager.Data.ScriptData[scriptKey], false);
        StartCoroutine(QuitScene(script));
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

        quitScript.Init(GameManager.Data.ScriptData["낙인소_퇴장"], false);

        yield return StartCoroutine(quitScript.PrintScript());
        SceneChanger.SceneChange("StageSelectScene");
    }
}