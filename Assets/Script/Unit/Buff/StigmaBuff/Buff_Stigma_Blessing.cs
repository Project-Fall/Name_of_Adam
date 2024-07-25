using UnityEngine;

public class Buff_Stigma_Blessing : Buff
{
    int _mana = 0;

    public override void Init(BattleUnit owner)
    {
        _buffEnum = BuffEnum.Blessing;

        _name = "Blessing";

        _description = "Blessing Info";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.AFTER_ATTACK;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = true;
    }

    public override bool Active(BattleUnit caster)
    {
        BattleManager.Mana.ChangeMana(_mana);

        return false;
    }

    public override void SetValue(int num)
    {
        _mana += num;
    }
}