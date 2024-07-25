using UnityEngine;

public class Buff_Stigma_Regeneration : Buff
{
    int hpUp = 0;
    public override void Init(BattleUnit owner)
    {
        _buffEnum = BuffEnum.Regeneration;

        _name = "Regeneration";

        _description = "Regeneration Info";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.ATTACK_TURN_END;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = true;
    }

    public override bool Active(BattleUnit caster)
    {
        _owner.GetHeal(hpUp, caster);

        return false;
    }

    public override void SetValue(int num)
    {
        hpUp += num;
    }
}