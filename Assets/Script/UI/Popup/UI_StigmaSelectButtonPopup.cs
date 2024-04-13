using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_StigmaSelectButtonPopup : UI_Popup
{
    [SerializeField] private UI_StigmaSelectButton _buttonPrefab;
    [SerializeField] private Transform _grid;
    [SerializeField] private TextMeshProUGUI _titleText;

    private Action _afterPopupAction;
    private DeckUnit _targetUnit;
    private StigmaInterface _sc;
    private int _stigmaCount;
    private bool _isCanReset;

    public void Init(DeckUnit targetUnit, string titleText, List<Stigma> stigmata = null, int stigmaCount = 0, Action afterPopupAction = null, StigmaInterface sc = null)
    {
        _afterPopupAction = afterPopupAction;
        _targetUnit = targetUnit;
        _sc = sc;
        _stigmaCount = stigmaCount;
        _isCanReset = (titleText.Equals("Select Stigma")) ? true : false;

        _titleText.SetText(GameManager.Locale.GetLocalizedEventScene(titleText));

        if (targetUnit == null)
        {
            SetStigmaSelectButtons(stigmata);
        }
        else
        {
            List<Stigma> stigmaList = CreateStigmaList(_targetUnit, stigmaCount);
            SetStigmaSelectButtons(stigmaList);
        }
    }

    private List<Stigma> CreateStigmaList(DeckUnit targetUnit, int stigmaCount)
    {
        List<Stigma> result = new();
        List<Stigma> existStigma = targetUnit.GetStigma();

        while (result.Count < stigmaCount)
        {
            Stigma stigma;
            if (_sc != null && _sc.GetType() == typeof(HarlotSceneController))
            {
                stigma = GameManager.Data.StigmaController.GetRandomHarlotStigma();
            }
            else
            {
                stigma = GameManager.Data.StigmaController.GetRandomStigmaAsUnit(new int[] { 99, 89 }, targetUnit.Data.name);
            }

            if (!existStigma.Contains(stigma) && !result.Contains(stigma))
                result.Add(stigma);
        }

        return result;
    }

    public void ResetStigmaSelectButtons()
    {
        if (!_isCanReset)
            return;

        List<Stigma> stigmaList = CreateStigmaList(_targetUnit, _stigmaCount);
        
        var buttons = _grid.GetComponentsInChildren<UI_StigmaSelectButton>();
        foreach (var button in buttons)
            Destroy(button.gameObject);

        SetStigmaSelectButtons(stigmaList);
    }

    private void SetStigmaSelectButtons(List<Stigma> stigmaList)
    {
        foreach (Stigma stigma in stigmaList)
        {
            GameObject.Instantiate(_buttonPrefab, _grid).GetComponent<UI_StigmaSelectButton>().Init(stigma, _sc, this);
        }
    }

    public void OnClick(Stigma stigma)
    {
        if (SceneChanger.GetSceneName() == "BattleScene")
        {
            GameObject.Find("@UI_Root").transform.Find("UI_StigmaSelectBlocker").gameObject.SetActive(false);
            BattleManager.Data.CorruptionPopups.RemoveAt(BattleManager.Data.CorruptionPopups.Count - 1);
            BattleManager.Data.IsCorruptionPopupOn = false;
        }
        
        if (_targetUnit != null)
            _targetUnit.AddStigma(stigma);

        GameManager.Sound.Play("UI/UpgradeSFX/UpgradeSFX");
        if (_afterPopupAction != null)
        {
            _afterPopupAction.Invoke();
        }

        GameManager.UI.ClosePopup();
    }

    public void QuitBtn()
    {
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        if (SceneChanger.GetSceneName() == "BattleScene")
        {
            GameObject.Find("@UI_Root").transform.Find("UI_StigmaSelectBlocker").gameObject.SetActive(true);
        }
        this.transform.SetAsFirstSibling();
        this.gameObject.SetActive(false);
    }
}