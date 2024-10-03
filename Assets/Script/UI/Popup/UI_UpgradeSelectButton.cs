using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_UpgradeSelectButton : UI_Base
{
    [SerializeReference] private Image _upgradeImage;
    [SerializeReference] private TextMeshProUGUI _upgradeName;
    [SerializeReference] private TextMeshProUGUI _upgradeDescription;
    [SerializeReference] private GameObject _frame;
    [SerializeReference] private GameObject _goldFrame;
    [SerializeReference] private Button _button;

    readonly Color _goldTextColor = new(0.82f, 0.65f, 0.27f);
    readonly Color _orangeTextColor = new(1f, 0.55f, 0f);

    private UI_UpgradeSelectButtonPopup _popup;
    private Upgrade _upgrade;
    private int _selectIndex;

    public void Init(Upgrade upgrade, int selectIndex, UI_UpgradeSelectButtonPopup popup)
    {
        _popup = popup;
        _selectIndex = selectIndex;
        _upgrade = upgrade;
        _button.onClick.AddListener(OnClick);

        _upgradeImage.sprite = upgrade.UpgradeImage160;

        if (upgrade.UpgradeData.Rarity == 2)
            _upgradeName.color = _goldTextColor;
        else if (upgrade.UpgradeData.Rarity == 3)
            _upgradeName.color = _orangeTextColor;

        _upgradeName.text = GameManager.Locale.GetLocalizedUpgrade(upgrade.UpgradeName);

        _upgradeDescription.SetText(GameManager.Data.UpgradeController.GetUpgradeDescription(upgrade));
        _goldFrame.SetActive(upgrade.UpgradeData.Rarity > 1);
    }

    public void OnHoverEnter()
    {
        _frame.GetComponent<Image>().color = new(0.8f, 0.8f, 0.8f);
        _goldFrame.GetComponent<Image>().color = new(0.8f, 0.8f, 0.8f);
    }

    public void OnHoverExit()
    {
        _frame.GetComponent<Image>().color = new(1f, 1f, 1f);
        _goldFrame.GetComponent<Image>().color = new(1f, 1f, 1f);
    }

    public void OnClick()
    {
        GameManager.Sound.Play("UI/UISFX/UIButtonSFX");
        _popup.OnClickUpgradeButton(_selectIndex);
    }
}
