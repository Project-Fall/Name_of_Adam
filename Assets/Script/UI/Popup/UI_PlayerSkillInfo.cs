using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerSkillInfo : UI_Scene
{
    [SerializeField] private GameObject _info;

    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _name;

    [SerializeField] private TextMeshProUGUI _statCost;
    [SerializeField] private TextMeshProUGUI _statDarkEssenceCost;

    [SerializeField] private TextMeshProUGUI _statCostChange;

    [SerializeField] private TextMeshProUGUI _skillDescription;

    //색상은 UI에서 정해주는대로
    readonly Color _upColor = new(1f, 0.22f, 0.22f);
    readonly Color _downColor = new(0.35f, 0.35f, 1f);

    public void SetInfo(PlayerSkill skill)
    {
        _name.text = skill.GetName();
        //_skillImage.sprite = skill.GetSkillFrameImage();

        _statCost.text = skill.GetManaCost().ToString();
        _statDarkEssenceCost.text = skill.GetDarkEssenceCost().ToString();

        if (BattleManager.PlayerSkillController.IsManaFree)
        {
            _statCostChange.gameObject.SetActive(true);
            _statCostChange.text = (skill.GetManaCost() - skill.GetOriginalManaCost()).ToString();
            _statCostChange.color = _upColor;
        }

        _skillDescription.text = skill.GetDescription();

        GetComponent<Canvas>().sortingOrder = 1;
    }

    public void SetPosition(Vector3 position)
    {
        _info.transform.position = position;
    }
}
