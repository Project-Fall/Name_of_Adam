using UnityEngine;

public class Stigma_Glory : Stigma
{
    public override void Use(BattleUnit caster)
    {
        base.Use(caster);

        caster.SetBuff(new Buff_Stigma_Glory());

        if (caster.Team == Team.Enemy)
        {
            Buff_EliteStatBuff statBuff = new();

            statBuff.SetValue(3);

            Stat buffedStat = new();

            buffedStat.MaxHP = 45;
            buffedStat.CurrentHP = 45;

            buffedStat.ATK = 10;

            statBuff.SetStat(buffedStat);

            caster.SetBuff(statBuff);
        }
    }
}