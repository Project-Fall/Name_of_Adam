using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public DeckUnit DeckUnit;
    public UnitDataSO Data => DeckUnit.Data;

    [SerializeField] public Stat BattleUnitChangedStat;//버프 등으로 변경된 스탯
    public Stat BattleUnitTotalStat => DeckUnit.DeckUnitTotalStat + BattleUnitChangedStat; //실제 적용 중인 스탯

    [SerializeField] private Team _team;
    public Team Team => _team;

    private SpriteRenderer _renderer;
    private Animator _unitAnimator;
    public AnimationClip SkillEffectAnim;

    [SerializeField] public UnitAIController AI;
    [SerializeField] public UnitHP HP;
    [SerializeField] public UnitFall Fall;
    [SerializeField] public UnitBuff Buff;
    [SerializeField] public UnitAction Action;
    [SerializeField] private UI_HPBar _hpBar;

    private bool _nextMoveSkip = false;
    private bool _nextAttackSkip = false;

    [SerializeField] public List<Stigma> StigmaList => DeckUnit.Stigma;

    [SerializeField] Vector2 _location;
    public Vector2 Location => _location;

    private float _scale;
    private float GetModifiedScale() => _scale + ((_scale * 0.1f) * (Location.y - 1));
    
    public void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _unitAnimator = GetComponent<Animator>();

        _renderer.sprite = Data.Image;

        AI.SetCaster(this);

        HP.Init(BattleUnitTotalStat.MaxHP, BattleUnitTotalStat.CurrentHP);
        Fall.Init(BattleUnitTotalStat.FallCurrentCount, BattleUnitTotalStat.FallMaxCount);

        Action.Init(this);
        _hpBar.RefreshHPBar(HP.FillAmount());

        _scale = transform.localScale.x;

        DeckUnit.SetStigma();
        BattleManager.Data.BattleUnitAdd(this);

        GameManager.Sound.Play("Summon/SummonSFX");
    }

    public void UnitSetting(Vector2 coord, Team team)
    {
        SetTeam(team);
        BattleManager.Field.EnterTile(this, coord);

        //소환 시 체크
        ActiveTimingCheck(ActiveTiming.SUMMON);
    }

    public void TurnStart()
    {
        //턴 시작 시 체크
        ActiveTimingCheck(ActiveTiming.TURN_START);
    }

    public void TurnEnd()
    {
        //턴 종료 시 체크
        ActiveTimingCheck(ActiveTiming.TURN_END);
    }

    public void MoveTurn()
    {
        //이동 턴 시작 시 체크
        _nextMoveSkip = ActiveTimingCheck(ActiveTiming.MOVE_TURN);
    }

    public void AttackTurn()
    {
        //공격 턴 시작 시 체크
        _nextAttackSkip = ActiveTimingCheck(ActiveTiming.ATTACK_TURN);
    }

    public void SetTeam(Team team)
    {
        _team = team;

        // 적군일 경우 x축 뒤집기
        _renderer.flipX = (Team == Team.Enemy) ? true : false;
        SetHPBar();
        ChangeAnimator();
    }

    public void SetHPBar()
    {
        _hpBar.SetHPBar(_team, transform);
        _hpBar.SetFallBar(DeckUnit);
    }

    public void SetLocate(Vector2 coord, bool move) {
        if (move)
        {
            ActiveTimingCheck(ActiveTiming.MOVE);
        }
            _location = coord;
    }
    
    public void UnitDiedEvent()
    {
        //자신이 사망 시 체크
        if (ActiveTimingCheck(ActiveTiming.UNIT_DEAD))
        {
            return;
        }

        GameManager.VisualEffect.StartVisualEffect(Resources.Load<AnimationClip>("Animation/UnitDeadEffect"), this.transform.position);

        StartCoroutine(UnitDeadEffect());

        if (_team == Team.Enemy)
        {
            GameManager.Data.DarkEssenseChage(Data.DarkEssenseDrop);
        }
        GameManager.Sound.Play("Dead/DeadSFX");
    }

    private IEnumerator UnitDeadEffect()
    {
        BattleManager.Data.BattleUnitRemove(this);
        BattleManager.Data.BattleOrderRemove(this);

        Color c = _renderer.color;

        while (c.a > 0)
        {
            c.a -= 0.01f;

            _renderer.color = c;

            yield return null;
        }

        Destroy(this.gameObject);
    }

    public void UnitFallEvent()
    {
        //타락 시 체크
        if (ActiveTimingCheck(ActiveTiming.FALLED))
        {
            return;
        }

        //타락 이벤트 시작
        HP.Init(DeckUnit.DeckUnitTotalStat.MaxHP, DeckUnit.DeckUnitTotalStat.MaxHP);
        BattleManager.Data.CorruptUnits.Add(this);

        GameManager.Sound.Play("UI/FallSFX/Fall");
        GameManager.VisualEffect.StartCorruptionEffect(this, transform.position);
    }

    public void Corrupted()
    {
        //타락 이벤트 종료
        BattleManager.Data.CorruptUnits.Remove(this);

        if (ChangeTeam() == Team.Enemy)
        {
            Fall.Editfy();
        }
        HP.Init(DeckUnit.DeckUnitTotalStat.MaxHP, DeckUnit.DeckUnitTotalStat.MaxHP);
        _hpBar.SetHPBar(Team, transform);
        _hpBar.RefreshHPBar(HP.FillAmount());

        DeckUnit.DeckUnitChangedStat.CurrentHP = 0;
        DeckUnit.DeckUnitUpgradeStat.FallCurrentCount = 0;
        BattleManager.Instance.BattleOverCheck();
    }

    public void AnimAttack()
    {
        StartCoroutine(BattleManager.Instance.UnitAttack());
    }

    public Team ChangeTeam(Team team = default)
    {
        if(team != default)
        {
            SetTeam(team);
            return team;
        }

        if (Team == Team.Player)
        {
            SetTeam(Team.Enemy);
            return Team.Enemy;
        }
        else
        {
            SetTeam(Team.Player);
            return Team.Player;
        }
    }

    private void ChangeAnimator()
    {
        if(Team == Team.Player)
        {
            _unitAnimator.runtimeAnimatorController = Data.CorruptionAnimatorController;
            if (Data.CorruptionSkillEffectAnim != null)
                SkillEffectAnim = Data.CorruptionSkillEffectAnim;
        }
        else
        {
            _unitAnimator.runtimeAnimatorController = Data.AnimatorController;
            if (Data.SkillEffectAnim != null)
                SkillEffectAnim = Data.SkillEffectAnim;
        }
    }

    public void SetPosition(Vector3 dest)
    {
        float s = GetModifiedScale();

        transform.position = dest;
        transform.localScale = new Vector3(s, s, 1);
    }

    public IEnumerator MovePosition(Vector3 dest)
    {
        float moveTime = 0.4f;
        float time = 0;
        Vector3 curP = transform.position;
        Vector3 curS = transform.localScale;

        float addScale = GetModifiedScale();

        while (time < moveTime)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(curP, dest, time / moveTime);
            transform.localScale = Vector3.Lerp(curS, new Vector3(addScale, addScale, 1), time / moveTime);
            yield return null;
        }
    }

    public void Attack(BattleUnit unit) {
        if(unit != null)
        {
            bool attackSkip = false;
            Team team = unit.Team;

            //공격 전 체크
            attackSkip |= ActiveTimingCheck(ActiveTiming.BEFORE_ATTACK, unit);

            //대미지 확정 시 체크
            attackSkip |= ActiveTimingCheck(ActiveTiming.DAMAGE_CONFIRM, unit);

            if (team != unit.Team)
            {
                //타락시켰을 시 체크
                ActiveTimingCheck(ActiveTiming.FALL, unit);
                attackSkip = true;
            }

            if (attackSkip)
                return;

            unit.GetAttack(-BattleUnitTotalStat.ATK, this);

            //공격 후 체크
            ActiveTimingCheck(ActiveTiming.AFTER_ATTACK, unit);
        }
    }

    public void GetAttack(int value, BattleUnit caster)
    {
        //피격 전 체크
        if (ActiveTimingCheck(ActiveTiming.BEFORE_ATTACKED, caster))
        {
            return;
        }

        ChangeHP(value);

        //피격 후 체크
        ActiveTimingCheck(ActiveTiming.AFTER_ATTACKED, caster);
    }

    public void ChangeHP(int value) {
        HP.ChangeHP(value);
        DeckUnit.DeckUnitChangedStat.CurrentHP += value;
        _hpBar.RefreshHPBar(HP.FillAmount());
    }

    public void ChangeFall(int value)
    {
        Fall.ChangeFall(value);
        DeckUnit.DeckUnitUpgradeStat.FallCurrentCount += value;
        _hpBar.RefreshFallGauge(Fall.GetCurrentFallCount());
    }

    public void SetBuff(Buff buff, BattleUnit caster)
    {
        Buff.SetBuff(buff, caster);
        BattleUnitChangedStat = Buff.GetBuffedStat();
    }

    public bool ActiveTimingCheck(ActiveTiming activeTiming, BattleUnit receiver = null)
    {
        bool skipNextAction = false;

        foreach (Stigma stigma in StigmaList)
        {
            if (stigma.ActiveTiming == activeTiming)
            {
                stigma.Use(this, receiver);
            }
        }

        foreach (Buff buff in Buff.CheckActiveTiming(activeTiming))
        {
            skipNextAction = buff.Active(this, receiver);
        }

        Buff.CheckCountDownTiming(activeTiming);

        BattleUnitChangedStat = Buff.GetBuffedStat();

        return skipNextAction;
    }

    public bool GetFlipX() => _renderer.flipX;

    public CutSceneType GetCutSceneType() => CutSceneType.center; // Skill 없어져서 바꿨어요

    public List<Vector2> GetAttackRange()
    {
        List<Vector2> RangeList = new();

        if (_nextAttackSkip)
        {
            _nextAttackSkip = false;
            return RangeList;
        }

        int Acolumn = 11;
        int Arow = 5;

        for (int i = 0; i < Data.AttackRange.Length; i++)
        {
            if (Data.AttackRange[i])
            {
                int x = (i % Acolumn) - (Acolumn >> 1);
                int y = (i / Acolumn) - (Arow >> 1);

                Vector2 vec = new(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }

    public List<Vector2> GetMoveRange()
    {
        List<Vector2> RangeList = new();

        if (_nextMoveSkip)
        {
            _nextMoveSkip = false;
            return RangeList;
        }

        int Mrow = 5;
        int Mcolumn = 5;

        for (int i = 0; i < Data.MoveRange.Length; i++)
        {
            if (Data.MoveRange[i])
            {
                int x = (i % Mcolumn) - (Mcolumn >> 1);
                int y = -((i / Mcolumn) - (Mrow >> 1));

                Vector2 vec = new(x, y);

                RangeList.Add(vec);
            }
        }

        return RangeList;
    }

    public List<Vector2> GetSplashRange(Vector2 target, Vector2 caster)
    {
        List<Vector2> SplashList = new List<Vector2>();

        int Scolumn = 11;
        int Srow = 5;

        for (int i = 0; i < Data.SplashRange.Length; i++)
        {
            if (Data.SplashRange[i])
            {
                int x = (i % Scolumn) - (Scolumn >> 1);
                int y = (i / Scolumn) - (Srow >> 1);

                if ((target - caster).x > 0) SplashList.Add(new Vector2(x, y)); //오른쪽
                else if ((target - caster).x < 0) SplashList.Add(new Vector2(-x, y)); //왼쪽
                else if ((target - caster).y > 0) SplashList.Add(new Vector2(y, x)); //위쪽
                else if ((target - caster).y < 0) SplashList.Add(new Vector2(-y, x)); //아래쪽
            }
        }
        return SplashList;
    }
}