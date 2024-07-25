using System;
using UnityEngine;

public class Stigma_Regeneration : Stigma
{
    public override void Use(BattleUnit caster)
    {
        base.Use(caster);

        Buff_Stigma_Regeneration regeneration = new();
        if (caster.Buff.CheckBuff(BuffEnum.Regeneration))
            regeneration = caster.Buff.GetBuff(BuffEnum.Regeneration) as Buff_Stigma_Regeneration;

        if (Tier == StigmaTier.Tier1)
        {
            regeneration.SetValue(5);
        }
        else if (Tier == StigmaTier.Tier2)
        {
            regeneration.SetValue(10);
        }

        caster.SetBuff(regeneration);
    }
}
