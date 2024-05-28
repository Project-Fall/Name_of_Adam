using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Stigma_Defeasance : Buff
{
    public override void Init(BattleUnit owner)
    {
        _buffEnum = BuffEnum.Additional_Punishment;

        _name = "��ȿȭ";

        _description = "�ǰ� �� 20% Ȯ���� �� �������� ��ȿȭ�մϴ�.";

        _count = -1;

        _countDownTiming = ActiveTiming.BEFORE_ATTACKED;

        _buffActiveTiming = ActiveTiming.BEFORE_ATTACKED;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = true;
    }

    public override bool Active(BattleUnit caster)
    {
        if (RandomManager.GetFlag(0.2f))
            return true;
        else
            return false;
    }
}