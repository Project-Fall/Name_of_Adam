using System.Collections.Generic;
using UnityEngine;

public class Stigma_Encourage : Stigma
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        base.Use(caster, receiver);

        List<BattleUnit> targetUnits = BattleManager.Field.GetArroundUnits(caster.Location);

        foreach (BattleUnit unit in targetUnits)
        {
            if (unit.Team == caster.Team)
            {
                Buff_Encourage encourage = new();
                unit.SetBuff(encourage, caster);
                if (Tier == StigmaTier.Tier2)
                {
                    unit.SetBuff(encourage, caster);
                }
            }
        }
    }
}