using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

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

    [SerializeField] private TextMeshProUGUI _statHealth;
    [SerializeField] private TextMeshProUGUI _statAttack;
    [SerializeField] private TextMeshProUGUI _statSpeed;
    [SerializeField] private TextMeshProUGUI _statCost;

    [SerializeField] private GameObject _statDarkEssence;
    [SerializeField] private TextMeshProUGUI _statDarkEssenceCost;

    [SerializeField] private TextMeshProUGUI _statHealthChange;
    [SerializeField] private TextMeshProUGUI _statAttackChange;
    [SerializeField] private TextMeshProUGUI _statSpeedChange;
    [SerializeField] private TextMeshProUGUI _statCostChange;

    [SerializeField] private Transform _unitInfoStigmaGrid;
    [SerializeField] private Transform _unitInfoUpgradeCountGrid;
    [SerializeField] private GameObject _AddedUpgradeCountSocket;
    [SerializeField] private Transform _unitInfoSkillRangeGrid;
    [SerializeField] private Image _unitImage;

    [SerializeField] private Image _insignia;

    [SerializeField] private Image _apostleInsignia;
    [SerializeField] private Image _eliteInsignia;
    [SerializeField] private Image _bossInsignia;

    [SerializeField] private TextMeshProUGUI _attackType;
    //public AnimationClip _unitAnimationClip;

    private DeckUnit _unit;
    UnitDataSO Data => _unit.Data;
    private Action<DeckUnit> _onSelect;
    private Action _endEvent;
    private CurrentEvent _currentEvent = CurrentEvent.None;

    readonly Color _attackRangeColor = new(0.6f, 0.05f, 0.05f);
    readonly Color _unitRangeColor = new(0.77f, 0.45f, 0.45f);

    readonly Color _playerInsigniaColor = new(0.35f, 0.09f, 0.05f);
    readonly Color _enemyInsigniaColor = new(0.54f, 0.5f, 0.34f);

    readonly Color _upColor = new(1f, 0.22f, 0.22f);
    readonly Color _downColor = new(0.35f, 0.35f, 1f);

    public void SetUnit(DeckUnit unit)
    {
        _unit = unit;
    }

    public void Init(Action<DeckUnit> onSelect = null, CurrentEvent currentEvent = CurrentEvent.None, Action endEvent=null)
    {
        _unitImage.sprite = _unit.Data.CorruptImage;
        _insignia.color = _playerInsigniaColor;

        _apostleInsignia.gameObject.SetActive(_unit.Data.Rarity == Rarity.Original);
        _eliteInsignia.gameObject.SetActive(_unit.Data.Rarity == Rarity.Elite);
        _bossInsignia.gameObject.SetActive(_unit.Data.Rarity == Rarity.Boss);

        _onSelect = onSelect;
        _endEvent = endEvent;

        _selectButton.SetActive(onSelect != null);

        _currentEvent = currentEvent;

        if (_currentEvent == CurrentEvent.Complete_Upgrade || _currentEvent == CurrentEvent.Complete_Heal_Faith
            || _currentEvent == CurrentEvent.Complate_Stigmata || _currentEvent == CurrentEvent.Complate_Apostle)
        {
            _quitButton.SetActive(false);
            _completeButton.SetActive(true);
        }
        else if (currentEvent == CurrentEvent.Hall_Delete)
        {
            _selectButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString(tableReference: "UITable", "Exile");
        }
        else if (_currentEvent == CurrentEvent.Stigmata_Full_Exception || _currentEvent == CurrentEvent.Upgrade_Full_Exception)
        {
            Select();
        }

        _unitInfoName.text = _unit.Data.Name;

        _statHealth.text = _unit.DeckUnitStat.MaxHP.ToString();
        _statAttack.text = _unit.DeckUnitStat.ATK.ToString();
        _statSpeed.text = _unit.DeckUnitStat.SPD.ToString();
        _statCost.text = _unit.DeckUnitStat.ManaCost.ToString();

        _statDarkEssence.SetActive(_unit.Data.DarkEssenseCost > 0);
        _statDarkEssenceCost.text = _unit.Data.DarkEssenseCost.ToString();

        _statHealthChange.text = (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP > 0) ? "+" : "";
        if (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP != 0)
        {
            _statHealthChange.text += (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP).ToString();
            _statHealthChange.color = (_unit.DeckUnitStat.MaxHP - _unit.Data.RawStat.MaxHP > 0) ? _upColor : _downColor;
        }

        _statAttackChange.text = (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK > 0) ? "+" : "";
        if (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK != 0)
        {
            _statAttackChange.text += (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK).ToString();
            _statAttackChange.color = (_unit.DeckUnitStat.ATK - _unit.Data.RawStat.ATK > 0) ? _upColor : _downColor;
        }

        _statSpeedChange.text = (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD > 0) ? "+" : "";
        if (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD != 0)
        {
            _statSpeedChange.text += (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD).ToString();
            _statSpeedChange.color = (_unit.DeckUnitStat.SPD - _unit.Data.RawStat.SPD > 0) ? _upColor : _downColor;
        }

        _statCostChange.text = (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost > 0) ? "+" : "";
        if (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost != 0)
        {
            _statCostChange.text += (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost).ToString();
            _statCostChange.color = (_unit.DeckUnitStat.ManaCost - _unit.Data.RawStat.ManaCost > 0) ? _downColor : _upColor;
        }

        for (int i = 0; i < 4; i++)
        {
            UI_FallUnit fu = GameObject.Instantiate(_fallGaugePrefab, _unitInfoFallGrid).GetComponent<UI_FallUnit>();
            int fallType = i / 4;

            fu.InitFall(Team.Player, fallType);
            if (i >= _unit.DeckUnitTotalStat.FallMaxCount - _unit.DeckUnitTotalStat.FallCurrentCount)
                fu.SetVisible(false);
        }

        List<Stigma> stigmataList = _unit.GetStigma();
        for (int i = 0; i < _unit.MaxStigmaCount; i++)
        {
            UI_HoverImageBlock ui = GameObject.Instantiate(_stigmaPrefab, _unitInfoStigmaGrid).GetComponent<UI_HoverImageBlock>();
            if (i < stigmataList.Count)
            {
                ui.Set(stigmataList[i].Sprite_88, "<size=150%>" + stigmataList[i].Name + "</size>" + "\n\n" + stigmataList[i].Description);
                ui.EnableUI(true);
            }
            else
                ui.EnableUI(false);
        }

        int createUpgradeCount = (GameManager.OutGameData.IsUnlockedItem(12)) ? 3 : 2;
        List<Upgrade> upgrades = _unit.DeckUnitUpgrade;
        for (int i = 0; i < createUpgradeCount; i++)
        {
            UI_HoverImageBlock ui = GameObject.Instantiate(_upgradeCountPrefab, _unitInfoUpgradeCountGrid).GetComponent<UI_HoverImageBlock>();
            if (i < upgrades.Count)
            {
                ui.Set(upgrades[i].UpgradeImage88, GameManager.Data.UpgradeController.GetUpgradeFullDescription(upgrades[i]));
                ui.EnableUI(true);
            }
            else
                ui.EnableUI(false);
        }

        for (int i = 0; i < _unit.Data.AttackRange.Length; i++)
        {
            Image block = GameObject.Instantiate(_squarePrefab, _unitInfoSkillRangeGrid).GetComponent<Image>();
            if (i == 27)
                block.color = _unitRangeColor;
            else if (_unit.Data.AttackRange[i])
                block.color = _attackRangeColor;
            else
                block.color = Color.grey;
        }

        _attackType.text = GameManager.Locale.GetLocalizedBattleScene(_unit.Data.UnitAttackType.ToString());

        if (GameManager.OutGameData.IsUnlockedItem(12))
        {
            _AddedUpgradeCountSocket.SetActive(true);
        }
    }

    public void SetAnimation()
    {
        AnimationClip clip = Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitSpawnBackEffect");
        GameManager.VisualEffect.StartVisualEffect(clip, new Vector3(0f, 3.5f, 0f));
    }

    public void Quit()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        GameManager.UI.ClosePopup();
    }

    public void Select()
    {
        if (CurrentSceneName().Equals("EventScene") && _currentEvent == CurrentEvent.Heal_Faith_Select)
        {
            GameManager.Sound.Play("UI/UISFX/UIImportantButtonSFX");
        }
        else
        {
            GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        }

        if (_onSelect != null)
        {
            switch (_currentEvent)
            {
                case CurrentEvent.Stigmata_Select:
                    if (_unit.GetStigmaCount() == 3)
                    {
                        GameManager.UI.ShowPopup<UI_SystemSelect>().Init("StigmaMax", () =>
                        {
                            GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

                            _onSelect(_unit);
                            _quitButton.SetActive(false);
                            _selectButton.SetActive(false);
                            _eventButton.SetActive(true);
                        });
                        return;
                    }
                    break;

                case CurrentEvent.Upgrade_Select:
                    if (_unit.DeckUnitUpgrade.Count == 3 || (_unit.DeckUnitUpgrade.Count == 2 && !GameManager.OutGameData.IsUnlockedItem(12)))
                    {
                        GameManager.UI.ShowPopup<UI_SystemSelect>().Init("UpgradeMax", () =>
                        {
                            GameManager.Sound.Play("UI/UISFX/UIButtonSFX");

                            _onSelect(_unit);
                            _quitButton.SetActive(false);
                            _selectButton.SetActive(false);
                            _eventButton.SetActive(true);
                        });
                        return;
                    }
                    break;
                case CurrentEvent.Revert_Unit_Select:
                    UI_MyDeck myDeck = FindObjectOfType<UI_MyDeck>();
                    myDeck.SelectCard(_unit);

                    break;
            }

            _onSelect(_unit);
        }

        if (CurrentSceneName().Equals("EventScene") && _currentEvent != CurrentEvent.Revert_Unit_Select)
        {
            _quitButton.SetActive(false);
            _selectButton.SetActive(false);
            _eventButton.SetActive(true);
        }
    }

    public void EventButtonClick()
    {
        switch (_currentEvent)
        {
            case CurrentEvent.Stigmata_Full_Exception:
            case CurrentEvent.Upgrade_Select:
            case CurrentEvent.Upgrade_Full_Exception:
            case CurrentEvent.Stigmata_Select:
            case CurrentEvent.Stigmata_Give:
            case CurrentEvent.Stigmata_Receive:
            case CurrentEvent.Corrupt_Stigmata_Select:
                GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
                Transform e = this.transform.parent.GetChild(0);
                e.SetAsLastSibling();
                e.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void CompleteButtonClick()
    {
        if (_currentEvent == CurrentEvent.Complate_Apostle && _onSelect != null)
            _onSelect(_unit);

        gameObject.SetActive(false);
        if (_endEvent != null)
            _endEvent.Invoke();

        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
    }

    public void UIInsigniaHover()
    {
        if (_unit.Data.Rarity == Rarity.Original)
            UIMouseEnter("Apostle");
        else if (_unit.Data.Rarity == Rarity.Elite)
            UIMouseEnter("Elite");
        else if (_unit.Data.Rarity == Rarity.Boss)
            UIMouseEnter("Boss");
        else
            UIMouseEnter("Normal");
    }

    public void UIAttackRangeHover()
    {
        if (_unit.Data.UnitAttackType == UnitAttackType.SingleAttack)
            UIMouseEnter("SingleAttackHover");
        else if (_unit.Data.UnitAttackType == UnitAttackType.AreaAttack)
            UIMouseEnter("AreaAttackHover");
        else if (_unit.Data.UnitAttackType == UnitAttackType.FrontalAttack)
            UIMouseEnter("FrontalAttackHover");
        else if (_unit.Data.UnitAttackType == UnitAttackType.SpecialAttack)
            UIMouseEnter("SpecialAttackHover");
        else if (_unit.Data.UnitAttackType == UnitAttackType.NoAttack)
            UIMouseEnter("NoAttackHover");
    }

    private bool _isHover = false;
    private bool _isHoverMessegeOn = false;
    private string _hoverText;

    public void UIMouseEnter(string key)
    {
        _isHover = true;
        _hoverText = key;
        GameManager.Instance.PlayAfterCoroutine(() =>
        {
            if (_isHover && !_isHoverMessegeOn && key == _hoverText)
            {
                _isHoverMessegeOn = true;
                GameManager.UI.ShowHover<UI_TextHover>().SetText(
                    $"{GameManager.Locale.GetLocalizedUI(key)}", Input.mousePosition);
            }
        }, 0.8f);
    }

    public void UIMouseExit()
    {
        _isHover = false;

        if (_isHoverMessegeOn)
        {
            _isHoverMessegeOn = false;
            GameManager.UI.CloseHover();
        }
    }


    public override bool ESCAction()
    {
        Debug.Log(_currentEvent);
        if (_currentEvent == CurrentEvent.Complate_Apostle || 
            _currentEvent == CurrentEvent.Complate_Stigmata ||
            _currentEvent == CurrentEvent.Complete_Upgrade ||
            _currentEvent == CurrentEvent.Complete_Heal_Faith)
            return false;

        GameManager.UI.ClosePopup();
        GameManager.Sound.Play("UI/UISFX/UICloseSFX");

        return true;
    }
}
