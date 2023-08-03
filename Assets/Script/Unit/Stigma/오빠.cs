using System.Collections.Generic;
using UnityEngine;

public class 오빠 : Stigma
{
    private 동생 _connectedPassive; // 연결된 동생 낙인
    private List<BattleUnit> _pointedUnits = new List<BattleUnit>(); // 해의 증표 붙은 애들

    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        if (_connectedPassive == null)
            FindConnectPassive();

        if (_connectedPassive == null)
            return;

        base.Use(caster, receiver);

        List<BattleUnit> units = _connectedPassive.GetPointedUnits();
        if (units.Contains(receiver))
        {
            receiver.ChangeFall(1);
            caster.ChangeHP(10);
            _connectedPassive.RemoveMoonMark(receiver);
        }

        AddSunMark(receiver);
    }

    // 필드 위에 동생 낙인 있나 확인
    private void FindConnectPassive()
    {
        foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
        {
            if (unit.DeckUnit.Data.Name.Equals("남매-여동생") == false)
                continue;

            foreach (Stigma passive in unit.DeckUnit.Stigma)
                if (passive.GetType() == typeof(동생))
                {
                    _connectedPassive = passive as 동생;
                    Debug.Log("오빠 낙인 등록");
                    return;
                }
        }
    }

    public List<BattleUnit> GetPointedUnits()
    {
        return _pointedUnits;
    }

    public void RemoveSunMark(BattleUnit unit)
    {
        _pointedUnits.Remove(unit);
    }

    public void AddSunMark(BattleUnit unit)
    {
        Debug.Log($"{unit.Data.Name}에 해의 증표");
        _pointedUnits.Add(unit);
    }
}