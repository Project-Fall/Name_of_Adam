using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    protected BuffEnum _buffEnum;
    public BuffEnum BuffEnum => _buffEnum;

    protected string _name;
    public string Name => _name;

    protected Sprite _sprite;
    public Sprite Sprite => _sprite;

    protected string _description;
    public string Description => _description;

    protected int _count;
    public int Count => _count;

    protected ActiveTiming _countDownTiming;
    public ActiveTiming CountDownTiming => _countDownTiming;

    protected ActiveTiming _buffActiveTiming;
    public ActiveTiming BuffActiveTiming => _buffActiveTiming;

    protected bool _statBuff;
    public bool StatBuff => _statBuff;

    protected bool _dispellable;
    public bool Dispellable => _dispellable;

    protected bool _caster;
    public bool Caster => _caster;


    public abstract void Init(BattleUnit caster);
    public abstract bool Active(BattleUnit caster = null, BattleUnit rereceiver = null);

    public abstract void Stack();

    public abstract Stat GetBuffedStat();

    public void CountChange(int num)
    {
        _count += num;
    }
}