using UnityEngine;

public class Buff_Crime : Buff
{
    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Crime;

        _name = "죄악";

        _description = "공격 시 타락을 1 부여합니다.";

        _count = 2;

        _countDownTiming = ActiveTiming.BEFORE_ATTACK;

        _buffActiveTiming = ActiveTiming.BEFORE_ATTACK;

        _statBuff = false;

        _dispellable = true;

        _caster = caster;

        _owner = owner;
    }

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        receiver.ChangeFall(1);

        return false;
    }

    public override void Stack()
    {
        _count += 2;
    }

    public override Stat GetBuffedStat()
    {
        Stat stat = new();
        return stat;
    }

    public override void SetValue(int num)
    {
        
    }
}