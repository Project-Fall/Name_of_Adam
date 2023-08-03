using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    private BattleUnit _unit;
    public void Init(BattleUnit unit)
    {
        _unit = unit;
    }

    public void Action(BattleUnit receiver)
    {
        _unit.Attack(receiver, _unit.BattleUnitTotalStat.ATK);
    }
}