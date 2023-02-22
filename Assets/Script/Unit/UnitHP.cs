using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnitHP : MonoBehaviour
{
    [SerializeField] private int _maxHP;
    [SerializeField, ReadOnly] private int _currentHP;

    [Header("사망 이벤트")]
    public UnityEvent UnitDiedEvent;

    public void Init(int maxHP)
    {
        _maxHP = maxHP;
        _currentHP = maxHP;
    }

    public void ChangeHP(int value)
    {
        _currentHP += value;

        if(_currentHP <= 0)
        {
            _currentHP = 0;
            UnitDiedEvent.Invoke();
        }
        else if (_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }

        // Add HP Change Event
        Debug.Log("DMG : " + value + ", CurHP ; " + GetCurrentHP());
    }

    public int GetCurrentHP()
    {
        return _currentHP;
    }
}