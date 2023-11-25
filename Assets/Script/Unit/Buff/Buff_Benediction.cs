using UnityEngine;

public class Buff_Benediction : Buff
{
    GameObject benedictionEffect;
    public override void Init(BattleUnit owner)
    {
        _buffEnum = BuffEnum.Benediction;

        _name = "�ż�";

        _sprite = GameManager.Resource.Load<Sprite>($"Arts/Buff/Buff_Benediction_Sprite");

        _description = "���� �� ���� �ž��� 1 ����߸��ϴ�.";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.DAMAGE_CONFIRM;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = false;

        benedictionEffect = GameManager.VisualEffect.StartBenedictionEffect(_owner);
    }

    public override bool Active(BattleUnit caster)
    {
        caster.ChangeFall(1);

        return false;
    }

    public override void Destroy()
    {
        Destroy(benedictionEffect);
    }
}