using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sadism : Stigma
{
    private bool isApplied = false;

    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        if (isApplied)
            return;

        base.Use(caster, receiver);

        Buff_Sadism sadism = new();
        caster.SetBuff(sadism, caster);
        //caster.BattleUnitChangedStat.ATK += 3;
        isApplied = true;
    }
}