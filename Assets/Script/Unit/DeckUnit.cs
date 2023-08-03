using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DeckUnit
{
    public UnitDataSO Data; // 유닛 기초 정보
    
    [SerializeField] public Stat DeckUnitUpgradeStat; // 영구 변화 수치
    [SerializeField] public Stat DeckUnitChangedStat; // 일시적 변화 수치, 한 전투 내에서만 적용

    public Stat DeckUnitStat => Data.RawStat + DeckUnitUpgradeStat;//실제 스탯
    public Stat DeckUnitTotalStat => DeckUnitStat + DeckUnitChangedStat;//일시적 변경된 스탯

    [SerializeField] public List<Stigma> Stigma = new List<Stigma>();

    private int _maxStigmaCount = 3;

    public void SetStigma()
    {
        Stigma = Stigma.Distinct().ToList();

        foreach (Stigma stigma in Data.UniqueStigma)
            AddStigma(stigma);

        foreach (Stigma stigma in Stigma)
            AddStigma(stigma);
    }

    public void AddStigma(Stigma passive)
    {
        if (Stigma.Contains(passive))
        {
            Debug.Log($"이미 장착된 낙인입니다. : {passive.GetNameWithRomanNumber()}");
            return;
        }

        if(Stigma.Count >= _maxStigmaCount)
        {
            Debug.Log("최대 낙인 개수");
            return;
        }

        Stigma.Add(passive);
    }

    private int _firstTurnDiscount = 0;
    public void FirstTurnDiscount()
    {
        _firstTurnDiscount = (DeckUnitTotalStat.ManaCost + 1) / 2;
        DeckUnitChangedStat.ManaCost -= _firstTurnDiscount;
    }

    public void FirstTurnDiscountUndo()
    {
        DeckUnitChangedStat.ManaCost += _firstTurnDiscount;
    }
}