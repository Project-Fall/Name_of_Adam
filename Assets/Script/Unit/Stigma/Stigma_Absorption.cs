using UnityEngine;

public class Stigma_Absorption : Stigma
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        base.Use(caster, receiver);

        Buff_Absorption absorption = new();
        caster.SetBuff(absorption, caster);
    }
}