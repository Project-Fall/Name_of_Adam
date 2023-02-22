using System.Collections;
using UnityEngine;

public class Mana : MonoBehaviour
{
    // Mana Manage
    [SerializeField] private int _maxManaCost = 200;
    [ReadOnly, SerializeField] private int _currentMana = 0;
    private UI_ManaGauge _manaGuage;

    private void Awake()
    {
        _manaGuage = GameManager.UI.ShowScene<UI_ManaGauge>();
        ChangeMana(_maxManaCost);
    }

    public void ChangeMana(int value)
    {
        _currentMana += value;

        if (_maxManaCost <= _currentMana)
            _currentMana = _maxManaCost;
        else if (_currentMana < 0)
            _currentMana = 0;

        _manaGuage.DrawGauge(_maxManaCost, _currentMana);
    }

    public bool CanUseMana(int value)
    {
        if (_currentMana >= value)
            return true;

        Debug.Log("not enough mana");
        return false;
    }
}