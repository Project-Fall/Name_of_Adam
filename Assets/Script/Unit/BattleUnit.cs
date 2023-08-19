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

    //[SerializeField] public UnitAIController AI;
    [SerializeField] public UnitHP HP;
    [SerializeField] public UnitFall Fall;
    [SerializeField] public UnitBuff Buff;
    [SerializeField] public UnitAction Action;
    [SerializeField] private UI_HPBar _hpBar;
    [SerializeField] private UI_FloatingDamage _floatingDamage;

    [SerializeField] public List<Stigma> StigmaList => DeckUnit.Stigma;

    [SerializeField] Vector2 _location;
    public Vector2 Location => _location;

    private float _scale;
    private float GetModifiedScale() => _scale + ((_scale * 0.1f) * (Location.y - 1));

    private bool _nextMoveSkip = false;
    private bool _nextAttackSkip = false;

    private bool[] _moveRangeList;

    public void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _unitAnimator = GetComponent<Animator>();

        _renderer.sprite = Data.Image;

        HP.Init(BattleUnitTotalStat.MaxHP, BattleUnitTotalStat.CurrentHP);
        Fall.Init(BattleUnitTotalStat.FallCurrentCount, BattleUnitTotalStat.FallMaxCount);

        Action.Init(this);
        _hpBar.RefreshHPBar(HP.FillAmount());

        _scale = transform.localScale.x;

        _moveRangeList = new bool[Data.MoveRange.Length];
        Array.Copy(Data.MoveRange, _moveRangeList, Data.MoveRange.Length);

        DeckUnit.SetStigma();
        BattleManager.Data.BattleUnitList.Add(this);

        GameManager.Sound.Play("Summon/SummonSFX");
        GameManager.VisualEffect.StartBenedictionEffect(this);
    }

    public void UnitSetting(Vector2 coord, Team team)
    {
        SetTeam(team);
        BattleManager.Field.EnterTile(this, coord);

        //소환 시 체크
        ActiveTimingCheck(ActiveTiming.STIGMA);
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

    public void MoveTurnStart()
    {
        //이동 턴 시작 시 체크
        _nextMoveSkip = ActiveTimingCheck(ActiveTiming.MOVE_TURN_START);
        _nextMoveSkip |= ActiveTimingCheck(ActiveTiming.ACTION_TURN_START);
    }

    public void MoveTurnEnd()
    {
        //이동 턴 종료 시 체크
        ActiveTimingCheck(ActiveTiming.MOVE_TURN_END);
    }

    public void AttackTurnStart()
    {
        //공격 턴 시작 시 체크
        _nextAttackSkip = ActiveTimingCheck(ActiveTiming.ATTACK_TURN_START);
        _nextAttackSkip |= ActiveTimingCheck(ActiveTiming.ACTION_TURN_START);

    }

    public void AttackTurnEnd()
    {
        //공격 턴 종료 시 체크
        ActiveTimingCheck(ActiveTiming.ATTACK_TURN_END);
    }

    public void FieldUnitDdead()
    {
        //필드 유닛 사망시 체크
        ActiveTimingCheck(ActiveTiming.FIELD_UNIT_DEAD);
    }

    public void SetTeam(Team team)
    {
        _team = team;

        // 적군일 경우 x축 뒤집기
        SetFlipX(Team == Team.Enemy);
        SetHPBar();
        ChangeAnimator();
    }

    public void SetFlipX(bool flip)
    {
        _renderer.flipX = flip;
        _floatingDamage.DirectionChange(flip);
    }

    public bool GetFlipX() => _renderer.flipX;

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


        BattleManager.Instance.UnitDeadEvent(this);
        Debug.Log(Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitDeadEffect"));
        GameManager.VisualEffect.StartVisualEffect(Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitDeadEffect"), this.transform.position);
        StartCoroutine(UnitDeadEffect());
        GameManager.Sound.Play("Dead/DeadSFX");
    }

    private IEnumerator UnitDeadEffect()
    {
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
        GameManager.Sound.Play("UI/FallSFX/Fall");
        GameManager.VisualEffect.StartCorruptionEffect(this, transform.position);
    }

    public void Corrupted()
    {
        //타락 이벤트 종료
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
        ActiveTimingCheck(ActiveTiming.STIGMA);
    }

    //애니메이션에서 직접 실행시킴
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

    public IEnumerator MoveFieldPosition(Vector3 dest)
    {
        float moveTime = 0.2f;
        float backMoveTime = 0.5f;
        Vector3 overDistance = (dest - transform.position) * 0.1f;
        Vector3 pVel = Vector3.zero;
        Vector3 sVel = Vector3.zero;

        float addScale = GetModifiedScale();

        #region 감속 -> 등속
        
        while (Vector3.Distance(dest + overDistance, transform.position) >= 0.03f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, dest + overDistance, ref pVel, moveTime);
            transform.localScale = Vector3.SmoothDamp(transform.localScale, new Vector3(addScale, addScale, 1), ref sVel, moveTime);
            yield return null;
        }

        float time = 0;
        Vector3 vec = transform.position;
        while (time < backMoveTime)
        {
            time += Time.deltaTime;
            float t = time / backMoveTime;

            transform.position = Vector3.Lerp(vec, dest, t);

            yield return null;
        }
        
        #endregion

        #region 등속 -> 감속
        /*
        float time = 0;
        Vector3 vec = transform.position;
        while (time <= moveTime)
        {
            time += Time.deltaTime;
            float t = time / moveTime;

            transform.position = Vector3.Lerp(vec, dest + overDistance, t);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(addScale, addScale, 1), t);

            yield return null;
        }

        while(Vector3.Distance(dest, transform.position) >= 0.03f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, dest, ref pVel, backMoveTime);

            yield return null;
        }
        */
        #endregion

        #region 감속 -> 감속
        /*
        while (Vector3.Distance(dest + overDistance, transform.position) >= 0.03f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, dest + overDistance, ref pVel, moveTime);
            transform.localScale = Vector3.SmoothDamp(transform.localScale, new Vector3(addScale, addScale, 1), ref sVel, moveTime);
            yield return null;
        }

        pVel = Vector3.zero;

        while (Vector3.Distance(dest, transform.position) >= 0.03f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, dest, ref pVel, backMoveTime);

            yield return null;
        }
        */
        #endregion


        transform.position = dest;
        transform.localScale = new Vector3(addScale, addScale, 1);
    }

    public IEnumerator AttackMove(Vector3 movePosition, float moveTime)
    {
        float time = 0;
        Vector3 originPosition = transform.position;

        while(time <= moveTime)
        {
            time += Time.deltaTime;
            float t = time / moveTime;

            transform.position = Vector3.Lerp(originPosition, movePosition, t);

            yield return null;
        }
    }

    //엑티브 타이밍에 대미지 바꿀 때용
    public int ChangedDamage = 0;

    public void Attack(BattleUnit unit, int damage) {
        if(unit != null)
        {
            ChangedDamage = damage;
            bool attackSkip = false;
            Team team = unit.Team;

            //공격 전 체크
            attackSkip |= ActiveTimingCheck(ActiveTiming.BEFORE_ATTACK, unit);

            //대미지 확정 시 체크
            attackSkip |= ActiveTimingCheck(ActiveTiming.DAMAGE_CONFIRM, unit, ChangedDamage);

            if (team != unit.Team)
            {
                //타락시켰을 시 체크
                ActiveTimingCheck(ActiveTiming.FALL, unit);
                ActiveTimingCheck(ActiveTiming.UNIT_TERMINATE, unit);

                attackSkip = true;
            }

            if (attackSkip)
                return;

            unit.GetAttack(-ChangedDamage, this);

            //공격 후 체크
            ActiveTimingCheck(ActiveTiming.AFTER_ATTACK, unit, ChangedDamage);

            if (unit.HP.GetCurrentHP() <= 0)
            {
                ActiveTimingCheck(ActiveTiming.UNIT_KILL, unit);
                ActiveTimingCheck(ActiveTiming.UNIT_TERMINATE, unit);
            }

            ChangedDamage = 0;
        }
    }

    public void GetAttack(int value, BattleUnit caster)
    {
        //피격 전 체크
        if (ActiveTimingCheck(ActiveTiming.BEFORE_ATTACKED, caster))
        {
            return;
        }


        GameManager.VisualEffect.StartVisualEffect(
            Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/HitEffect"),
            transform.position);


        _floatingDamage.Init(value);

        ChangeHP(value);
        
        //피격 후 체크
        ActiveTimingCheck(ActiveTiming.AFTER_ATTACKED, caster);
    }

    public void ChangeHP(int value) {
        DeckUnit.DeckUnitChangedStat.CurrentHP += value;
        HP.ChangeHP(value);
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
        Buff.SetBuff(buff, caster, this);
        BattleUnitChangedStat = Buff.GetBuffedStat();
    }

    private bool ActiveTimingCheck(ActiveTiming activeTiming, BattleUnit receiver = null, int num = 0)
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
            buff.SetValue(num);
            skipNextAction = buff.Active(this, receiver);
        }

        Buff.CheckCountDownTiming(activeTiming);

        BattleUnitChangedStat = Buff.GetBuffedStat();

        return skipNextAction;
    }

    public void AddMoveRange(bool[] rangeList)
    {
        for (int i = 0; i < _moveRangeList.Length; i++)
        {
            _moveRangeList[i] |= rangeList[i];
        }
    }

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

        for (int i = 0; i < _moveRangeList.Length; i++)
        {
            if (_moveRangeList[i])
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