using System;
using UnityEngine;

public class Stigma_Blessing : Stigma
{
    public override void Use(BattleUnit caster)
    {
        base.Use(caster);

        Buff_Stigma_Blessing blessing = new();
        caster.SetBuff(blessing);

        if (Tier == StigmaTier.Tier1)
        {
            blessing.SetValue(10);
        }
        else if (Tier == StigmaTier.Tier2)
        {
            blessing.SetValue(15);
        }
    }
}
