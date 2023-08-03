using UnityEngine;

public class Buff_Encourage : Buff
{
    private int attackUp;
    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Encourage;

        _name = "고양";

        _description = "공격력이 50% 증가합니다.";

        _count = 1;

        _countDownTiming = ActiveTiming.DAMAGE_CONFIRM;

        _buffActiveTiming = ActiveTiming.NONE;

        _statBuff = true;

        _dispellable = true;

        _caster = caster;

        _owner = owner;

        attackUp = owner.DeckUnit.DeckUnitTotalStat.ATK / 2;
}

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        return false;
    }

    public override void Stack()
    {
        _count += 1;
    }

    public override Stat GetBuffedStat()
    {
        Stat stat = new();  
        stat.ATK += attackUp;

        return stat;
    }

    public override void SetValue(int num)
    {

    }
}