using System.Collections.Generic;
using UnityEngine;

public class Stigma_동생 : Stigma
{
    private Stigma_오빠 _connectedPassive; // 연결된 오빠 낙인
    private List<BattleUnit> _pointedUnits = new List<BattleUnit>(); // 달의 증표가 붙은 애들

    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        base.Use(caster, receiver);

        if (receiver.Buff.CheckBuff(BuffEnum.TraceOfSolar))
        {

        }
        else
        { 
            
        }
    }

    public void Use1(BattleUnit caster, BattleUnit receiver)
    {
        base.Use(caster, receiver);

        if (_connectedPassive == null)
            FindConnectPassive();

        if (_connectedPassive == null)
            return;

        List<BattleUnit> units = _connectedPassive.GetPointedUnits();

        foreach (BattleUnit unit in units)
        {
            unit.ChangeHP(-caster.BattleUnitTotalStat.ATK);
            unit.ChangeFall(1);
            AddMoonMark(unit);
        }
        _connectedPassive.GetPointedUnits().Clear();
    }

    private void FindConnectPassive()
    {
        foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
        {
            // 여기 이름 남매-오빠로 바꿔야 함
            if (unit.DeckUnit.Data.Name.Equals("검병") == false)
                continue;

            foreach (Stigma passive in unit.DeckUnit.Stigma)
                if (passive.GetType() == typeof(Stigma_오빠))
                {
                    _connectedPassive = passive as Stigma_오빠;
                    Debug.Log("동생 낙인 등록");
                    return;
                }
        }
    }

    public List<BattleUnit> GetPointedUnits()
    {
        return _pointedUnits;
    }

    public void RemoveMoonMark(BattleUnit unit)
    {
        _pointedUnits.Remove(unit);
    }

    public void AddMoonMark(BattleUnit unit)
    {
        Debug.Log($"{unit.Data.Name}에 달의 증표");
        _pointedUnits.Add(unit);
    }
}