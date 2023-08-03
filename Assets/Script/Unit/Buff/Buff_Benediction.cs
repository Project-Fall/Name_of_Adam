using UnityEngine;

public class Buff_Benediction : Buff
{    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Benediction;

        _name = "�ż�";

        _description = "������ ���� ������ ���ݿ��� ��ȭ�� 1 �ο��˴ϴ�.";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.BEFORE_ATTACK;

        _statBuff = true;

        _dispellable = false;

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