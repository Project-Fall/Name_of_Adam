using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuff : MonoBehaviour
{
    [SerializeField] private List<Buff> _buffList;

    public void SetBuff(Buff buff, BattleUnit caster)
    {
        buff.Init(caster);

        foreach (Buff listedBuff in _buffList)
        {
            if (buff.BuffEnum == listedBuff.BuffEnum)
            {
                listedBuff.Stack();

                return;
            }
        }
        
        _buffList.Add(buff);
    }

    public Stat GetBuffedStat()
    {
        Stat buffedStat = new();

        foreach (Buff buff in _buffList)
        {
            if (buff.StatBuff)
            {
                buffedStat += buff.GetBuffedStat();
            }
        }

        return buffedStat;
    }

    public List<Buff> CheckActiveTiming(ActiveTiming activeTiming)
    {
        List<Buff> checkedBuffList = new();

        foreach (Buff buff in _buffList)
        {
            if (buff.BuffActiveTiming == activeTiming)
            {
                checkedBuffList.Add(buff);
            }
        }

        return checkedBuffList;
    }

    public void CheckCountDownTiming(ActiveTiming countDownTiming)
    {
        int i;

        for (i = 0; i < _buffList.Count; i++)
        {
            if (_buffList[i].CountDownTiming == countDownTiming)
            {
                _buffList[i].CountChange(-1);
                if (_buffList[i].Count == 0)
                {
                    _buffList.Remove(_buffList[i]);
                    i--;
                }
            }
        }
    }
}