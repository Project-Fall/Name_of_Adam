using UnityEngine;

public class Buff_Bleeding : Buff
{
    public override void Init()
    {
        _buffEnum = BuffEnum.Bleeding;

        _name = "Bleeding";

        _description = "������ ���� 5�� ������� �Խ��ϴ�.";

        _count = 3;

        _countDownTiming = ActiveTiming.TURN_END;

        _buffActiveTiming = ActiveTiming.TURN_END;

        _passiveBuff = false;
}

    public override void Active(BattleUnit caster, BattleUnit receiver)
    {
        caster.ChangeHP(-5);
    }

    public override void Stack()
    {
        _count = 3;
    }

    public override Stat GetBuffedStat() 
    {
        Stat stat = new();
        return stat;
    }
}