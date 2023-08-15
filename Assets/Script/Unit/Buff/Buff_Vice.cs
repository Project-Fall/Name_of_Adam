using UnityEngine;

public class Buff_Vice : Buff
{
    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Vice;

        _name = "�˾�";

        _description = "���� �� Ÿ���� 1 �ο��մϴ�.";

        _count = 2;

        _countDownTiming = ActiveTiming.BEFORE_ATTACK;

        _buffActiveTiming = ActiveTiming.BEFORE_ATTACK;

        _caster = caster;

        _owner = owner;

        _statBuff = false;

        _dispellable = true;

        _stigmaBuff = false;
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
}