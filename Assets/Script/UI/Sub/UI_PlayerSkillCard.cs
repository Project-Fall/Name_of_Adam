using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_PlayerSkillCard : UI_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image _playerSkillImage;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _inactive;
    [SerializeField] private GameObject _skillCard;
    [SerializeField] private TextMeshProUGUI _ManaCost;
    [SerializeField] private TextMeshProUGUI _essenceCost;
    [SerializeField] private GameObject _essence;

    [SerializeField] private UI_CannotEffect _cannotEffect;
    public UI_CannotEffect CannotEffect => _cannotEffect;

    private UI_PlayerSkill _playerSkill;
    private PlayerSkill _skill;
    public bool IsSelected = false;

    private void Start()
    {
        _highlight.SetActive(false);
        _inactive.SetActive(false);
    }

    public void Set(UI_PlayerSkill ps, PlayerSkill skill)
    {
        _playerSkill = ps;
        _skill = skill;
        //_text.text = skill.GetName();
        _playerSkillImage.sprite = skill.GetSkillImage();
        _ManaCost.text = skill.GetManaCost().ToString();
        _essenceCost.text = skill.GetDarkEssenceCost().ToString();
        _cannotEffect.Init(Vector3.one, Vector3.one * 1.2f, 1.5f);

        if (skill.GetDarkEssenceCost() < 1)
        {
            _essenceCost.gameObject.SetActive(false);
            _essence.SetActive(false);
        }
    }

    private UI_PlayerSkillInfo _skillInfo;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _skillCard.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        //GameManager.UI.ShowHover<UI_SkillHover>().SetSkillHover(_skill.GetName(), _skill.GetManaCost(), _skill.GetDarkEssenceCost(), _skill.GetDescription(), eventData.position);

        _skillInfo = GameManager.UI.ShowScene<UI_PlayerSkillInfo>();
        _skillInfo.SetInfo(_skill);
        _skillInfo.SetPosition(this.transform.position + new Vector3(-100f,700f,0));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_skillInfo.gameObject);
        //GameManager.UI.CloseHover();
        if (IsSelected)
            return;
        _skillCard.transform.localScale = new Vector3(1f, 1f, 1f);

        //_highlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PhaseController phase = BattleManager.Phase;
        if (!phase.CurrentPhaseCheck(phase.Prepare))
        {
            GameManager.Sound.Play("UI/UISFX/UIFailSFX");
            BattleManager.BattleUI.UI_controlBar.CreateSystemInfo(GameManager.Locale.GetLocalizedSystem("CannotInUnitTurn"));
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (TutorialManager.Instance.IsEnableUpdate())
                TutorialManager.Instance.ShowNextTutorial();

            if (BattleManager.BattleUI.UI_playerSkill.Used)
            {
                //_cannotEffect.Create();
                BattleManager.BattleUI.UI_controlBar.CreateSystemInfo(GameManager.Locale.GetLocalizedSystem("PlayerSkillisUsed"));
            }
            _playerSkill.OnClickHand(this);
        }
    }

    public void ChangeSelectState(bool b)
    {
        IsSelected = b;
        _highlight.SetActive(b);

        if (!IsSelected)
        {
            _skillCard.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void ChangeInable(bool b)
    {
        _inactive.SetActive(b);
    }

    public PlayerSkill GetSkill() => _skill;
}
