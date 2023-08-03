using UnityEngine;

public class Buff_Encourage : Buff
{

    private int attackUp;
    public override void Init(BattleUnit caster)
    {
        _buffEnum = BuffEnum.Encourage;

        _name = "고양";

        _description = "공격력이 3 증가합니다.";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.NONE;

        _statBuff = true;

        _dispellable = true;

        _caster = caster;

        attackUp = 5;
}

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        return false;
    }

    public override void Stack()
    {
        attackUp += 5;
    }

    public override Stat GetBuffedStat()
    {
        Stat stat = new();
        stat.ATK += attackUp;

        return stat;
    }
}