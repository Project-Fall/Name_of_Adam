using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class UI_UnitInfo : UI_Popup
{
    [SerializeField] private GameObject _selectButton;
    [SerializeField] private GameObject _quitButton;
    [SerializeField] private TextMeshProUGUI _quitButtonTxt;
    [SerializeField] private GameObject _eventButton;
    [SerializeField] private GameObject _completeButton;
    [SerializeField] private GameObject _fallGaugePrefab;
    [SerializeField] private GameObject _stigmaPrefab;
    [SerializeField] private GameObject _upgradeCountPrefab;
    [SerializeField] private GameObject _squarePrefab;

    [SerializeField] private TextMeshProUGUI _unitInfoName;
    [SerializeField] private Transform _unitInfoFallGrid;
    [SerializeField] private TextMeshProUGUI _unitInfoStat;
    [SerializeField] private Transform _unitInfoStigmaGrid;
    [SerializeField] private Transform _unitInfoUpgradeCountGrid;
    [SerializeField] private GameObject _AddedUpgradeCountSocket;
    [SerializeField] private Transform _unitInfoSkillRangeGrid;
    [SerializeField] private Image _unitImage;

    //public AnimationClip _unitAnimationClip;

    private DeckUnit _unit;
    UnitDataSO Data => _unit.Data;
    private Action<DeckUnit> _onSelect;
    private Action _endEvent;
    private Action<DeckUnit> _selectRestorationUnit;
    private CUR_EVENT _evNum = 0;

    readonly string UpColorStr = "red";
    readonly string DownColorStr = "blue";

    public void SetUnit(DeckUnit unit)
    {
        _unit = unit;
    }

    public void Init(Action<DeckUnit> onSelect = null, CUR_EVENT eventNum = CUR_EVENT.NONE, Action endEvent=null)
    {
        _unitImage.sprite = _unit.Data.CorruptImage;

        _onSelect = onSelect;
        _endEvent = endEvent;

        _selectButton.SetActive(onSelect != null);

        _evNum = eventNum;

        if (_evNum == CUR_EVENT.COMPLETE_UPGRADE || _evNum == CUR_EVENT.COMPLETE_RELEASE
            || _evNum == CUR_EVENT.COMPLETE_STIGMA || _evNum == CUR_EVENT.COMPLETE_HAELOT)
        {
            _quitButton.SetActive(false);
            _completeButton.SetActive(true);
        }
        else if (_evNum == CUR_EVENT.STIGMA_EXCEPTION || _evNum == CUR_EVENT.UPGRADE_EXCEPTION)
        {
            Select();
        }

        _unitInfoName.text = _unit.Data.Name;

        string darkEssenseCost = (_unit.Data.DarkEssenseCost > 0) ? " / " + _unit.Data.DarkEssenseCost.ToString() : "";

        string hpChange = "";
        if (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP > 0)
        {
            hpChange = " <color=\"" + UpColorStr + "\">(+" + (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP).ToString() + ")</color>";
        }
        else if (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP < 0)
        {
            hpChange = " <color=\"" + DownColorStr + "\">(" + (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP).ToString() + ")</color>";
        }

        string costChange = "";
        if (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost > 0)
        {
            costChange = " <color=\"" + DownColorStr + "\">(+" + (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost).ToString() + ")</color>";
        }
        else if (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost < 0)
        {
            costChange = " <color=\"" + UpColorStr + "\">(" + (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost).ToString() + ")</color>";
        }

        string attackChange = "";
        if (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK > 0)
        {
            attackChange = " <color=\"" + UpColorStr + "\">(+" + (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK).ToString() + ")</color>";
        }
        else if (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK < 0)
        {
            attackChange = " <color=\"" + DownColorStr + "\">(" + (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK).ToString() + ")</color>";
        }

        string speedChange = "";
        if (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD > 0)
        {
            speedChange = " <color=\"" + UpColorStr + "\">(+" + (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD).ToString() + ")</color>";
        }
        else if (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD < 0)
        {
            speedChange = " <color=\"" + DownColorStr + "\">(" + (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD).ToString() + ")</color>";
        }

        _unitInfoStat.text = GameManager.Locale.GetLocalizedUpgrade("Cost") + ": " + _unit.DeckUnitTotalStat.MaxHP.ToString() + hpChange + "\n" +
                                    GameManager.Locale.GetLocalizedUpgrade("HP") + ": " + _unit.DeckUnitTotalStat.ManaCost.ToString() + costChange + darkEssenseCost + "\n" +
                                    GameManager.Locale.GetLocalizedUpgrade("Attack") + ": " + _unit.DeckUnitTotalStat.ATK.ToString() + attackChange + "\n" +
                                    GameManager.Locale.GetLocalizedUpgrade("Speed") + ": " + _unit.DeckUnitTotalStat.SPD.ToString() + speedChange;

        for (int i = 0; i < 4; i++)
        {
            UI_FallUnit fu = GameObject.Instantiate(_fallGaugePrefab, _unitInfoFallGrid).GetComponent<UI_FallUnit>();
            int fallType = i / 4;

            fu.InitFall(Team.Player, fallType);
            if (i >= _unit.DeckUnitTotalStat.FallMaxCount - _unit.DeckUnitTotalStat.FallCurrentCount)
                fu.SetVisible(false);
        }

        List<Stigma> stigmas = _unit.GetStigma();
        for (int i = 0; i < _unit._maxStigmaCount; i++)
        {
            UI_HoverImageBlock ui = GameObject.Instantiate(_stigmaPrefab, _unitInfoStigmaGrid).GetComponent<UI_HoverImageBlock>();
            if (i < stigmas.Count)
            {
                ui.Set(stigmas[i].Sprite_88, "<size=150%>" + stigmas[i].Name + "</size>" + "\n\n" + stigmas[i].Description);
                ui.EnableUI(true);
            }
            else
                ui.EnableUI(false);
        }

        foreach (Upgrade upgrade in _unit.DeckUnitUpgrade)
        {
            UI_HoverImageBlock ui = GameObject.Instantiate(_upgradeCountPrefab, _unitInfoUpgradeCountGrid).GetComponent<UI_HoverImageBlock>();
            ui.Set(upgrade.UpgradeImage88, "<size=150%>" + upgrade.UpgradeDescription + "</size>");
            ui.EnableUI(true);
        }

        foreach (bool range in _unit.Data.AttackRange)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _unitInfoSkillRangeGrid).GetComponent<Image>();
            if (range)
                block.color = Color.red;
            else
                block.color = Color.grey;
        }

        if (GameManager.OutGameData.IsUnlockedItem(12))
        {
            _AddedUpgradeCountSocket.SetActive(true);
        }
    }

    public void Restoration(Action<DeckUnit> OnSelect=null, CUR_EVENT Eventnum = CUR_EVENT.NONE,Action<DeckUnit> selectRestorationUnit=null)
    {
        this.Init(OnSelect,Eventnum);
        _selectRestorationUnit = selectRestorationUnit;
    } 

    public void SetAnimation()
    {
        AnimationClip clip = Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitSpawnBackEffect");
        GameManager.VisualEffect.StartVisualEffect(clip, new Vector3(0f, 3.5f, 0f));
    }

    public void Quit()
    {
        GameManager.Sound.Play("UI/ButtonSFX/BackButtonClickSFX");
        GameManager.UI.ClosePopup();
    }

    public void Select()
    {
        if (currentSceneName().Equals("EventScene") && _evNum == CUR_EVENT.RELEASE)
        {
            GameManager.Sound.Play("UI/ClickSFX/UIClick2");
        }
        else
        {
            GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        }

        if (_selectRestorationUnit != null)
        {
            _selectRestorationUnit(_unit);
        }

        if (_onSelect != null)
        {
            switch (_evNum)
            {
                case CUR_EVENT.STIGMA:
                    if (_unit.GetStigmaCount() == 3)
                    {
                        GameManager.UI.ShowPopup<UI_SystemSelect>().Init("StigmaMax", () => _onSelect(_unit));
                        return;
                    }
                    break;

                case CUR_EVENT.UPGRADE:
                    if (_unit.DeckUnitUpgrade.Count == 3 || (_unit.DeckUnitUpgrade.Count == 2 && !GameManager.OutGameData.IsUnlockedItem(12)))
                    {
                        GameManager.UI.ShowPopup<UI_SystemSelect>().Init("UpgradeMax", () => _onSelect(_unit));
                        return;
                    }
                    break;
            }

            _onSelect(_unit); 
        }

        if (currentSceneName().Equals("EventScene") && _evNum != CUR_EVENT.HARLOT_RESTORATION)
        {
            _quitButton.SetActive(false);
            _selectButton.SetActive(false);
            _eventButton.SetActive(true);
            
            //선택하기 버튼 없애고, 껏다켯다 버튼 만든다.
        }
    }

    public void EventButtonClick()
    {
        switch (_evNum)
        {
            case CUR_EVENT.STIGMA_EXCEPTION:
            case CUR_EVENT.UPGRADE:
            case CUR_EVENT.UPGRADE_EXCEPTION:
            case CUR_EVENT.STIGMA://강화하기, 스티그마 부여하기
            case CUR_EVENT.GIVE_STIGMA:
                Transform e = this.transform.parent.GetChild(0);
                e.SetAsLastSibling();
                e.gameObject.SetActive(true);
                break;
            case CUR_EVENT.RECEIVE_STIGMA:
                break;
            default:
                break;
        }
    }

    public void CompeleteButtonClick()
    {
        if (_evNum == CUR_EVENT.COMPLETE_HAELOT && _onSelect != null)
            _onSelect(_unit);
        gameObject.SetActive(false);
        GameManager.Sound.Play("UI/ButtonSFX/UIButtonClickSFX");
        if (_endEvent != null)
            _endEvent.Invoke();
    }
}