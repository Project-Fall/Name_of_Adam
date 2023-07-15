using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    private static BattleManager s_instance;
    public static BattleManager Instance { get { Init(); return s_instance; } }

    private SoundManager _sound;
    public static SoundManager Sound => Instance._sound;

    [SerializeField] BattleCutSceneController _battlecutScene;
    public static BattleCutSceneController BattleCutScene => Instance._battlecutScene;

    private BattleDataManager _battleData;
    public static BattleDataManager Data => Instance._battleData;

    private BattleUIManager _battleUI;
    public static BattleUIManager BattleUI => Instance._battleUI;

    private PlayerSkillController _playerSkillController;
    public static PlayerSkillController PlayerSkillController => Instance._playerSkillController;

    private Field _field;
    public static Field Field => Instance._field;

    private Mana _mana;
    public static Mana Mana => Instance._mana;

    private PhaseController _phase;
    public static PhaseController Phase => Instance._phase;

    private List<BattleUnit> hitUnits;
    private Vector2 coord;

    [SerializeField] List<GameObject> Background;

    public FieldColorType fieldColorType = FieldColorType.none;

    private void Awake()
    {
        _battleData = Util.GetOrAddComponent<BattleDataManager>(gameObject);
        _battleUI = Util.GetOrAddComponent<BattleUIManager>(gameObject);
        _mana = Util.GetOrAddComponent<Mana>(gameObject);
        _phase = new PhaseController();
        _playerSkillController = Util.GetOrAddComponent<PlayerSkillController>(gameObject);
        

        SetBackground();
    }

    private void Update()
    {
        _phase.OnUpdate();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@BattleManager");

            if (go == null)
            {
                //go = new GameObject("@BattleManager");
                //go.AddComponent<BattleManager>();
                return;
            }

            s_instance = go.GetComponent<BattleManager>();
        }
    }

    public void TurnStart()
    {
        foreach (BattleUnit unit in _battleData.BattleUnitList)
        {
            unit.TurnStart();
        }
    }

    public void TurnEnd()
    {
        foreach (BattleUnit unit in _battleData.BattleUnitList)
        {
            unit.TurnEnd();
        }
    }

    public void SetupField()
    {
        GameObject fieldObject = GameObject.Find("Field");

        if (fieldObject == null)
            fieldObject = GameManager.Resource.Instantiate("Field");


        _field = fieldObject.GetComponent<Field>();
    }

    public void SpawnInitialUnit()
    {
        GetComponent<UnitSpawner>().SpawnInitialUnit();
    }

    private void SetBackground()
    {
        string str = GameManager.Data.CurrentStageData.FactionName;

        for(int i = 0; i < 3; i++)
        {
            Background[i].gameObject.SetActive(false);
            //Background
            if (((Faction)i + 1).ToString() == str)
                Background[i].gameObject.SetActive(true);
        }

        if(str == "" || str == null)
            Background[0].gameObject.SetActive(true);
    }
    #region Click 관련

    public void MovePhase()
    {
        BattleUnit unit = Data.GetNowUnit();

        if (unit.Team == Team.Player)
        {
            if (Field.Get_Abs_Pos(unit, FieldColor.Move).Contains(coord) == false)
                return;
            Vector2 dest = coord - unit.Location;

            MoveLocate(unit, dest); //이동시 낙인 체크
        }

        _phase.ChangePhase(_phase.Action);
    }

    public void ActionPhase()
    {
        BattleUnit unit = Data.GetNowUnit();

        if (Field.Get_Abs_Pos(unit, FieldColor.Attack).Contains(coord) == false)
            return;

        if (coord != unit.Location)
        {
            List<Vector2> splashRange = unit.GetSplashRange(coord, unit.Location);
            List<BattleUnit> unitList = new List<BattleUnit>();

            foreach (Vector2 splash in splashRange)
            {
                BattleUnit targetUnit = Field.GetUnit(coord + splash);

                if (targetUnit == null)
                    continue;

                // 힐러의 예외처리 필요
                if(targetUnit.Team != unit.Team)
                    unitList.Add(targetUnit);
            }


            AttackStart(unit, unitList);
        }
    }

    public void EngagePhase()
    {
        Field.ClearAllColor();

        if (Data.OrderUnitCount <= 0)
        {
            _phase.ChangePhase(_phase.End);
            return;
        }

        _phase.ChangePhase(_phase.Move);
    }

    public void StartPhase()
    {
        if (Field._coloredTile.Count <= 0)
            return;

        if (fieldColorType == FieldColorType.UnitSpawn)
        {
            SpawnUnitOnField(true);
        }
    }

    public void PreparePhase()
    {
        if (Field._coloredTile.Contains(coord) == false)
            return;

        if (fieldColorType == FieldColorType.UnitSpawn)
        {
            SpawnUnitOnField();
        }
        else if (fieldColorType == FieldColorType.PlayerSkill)
        {
            BattleUI.UI_playerSkill.GetSelectedCard().GetSkill().Use(coord);
            PlayerSkillController.PlayerSkillUse();
        }
        else if (fieldColorType == FieldColorType.UltimatePlayerSkill)
        {
            if (GameManager.Data.PlayerSkillCountChage(-1))
            {
                BattleUI.UI_playerSkill.GetSelectedCard().GetSkill().Use(coord);
                PlayerSkillController.PlayerSkillUse();
            }
        }
    }

    private void SpawnUnitOnField(bool isFirst=false)
    {
        DeckUnit unit = BattleUI.UI_hands.GetSelectedUnit();
        if (Field._coloredTile.Contains(coord) == false)
            return;
        BattleUnit spawnedUnit = GetComponent<UnitSpawner>().DeckSpawn(unit, coord);
        Mana.ChangeMana(-unit.DeckUnitTotalStat.ManaCost);

        spawnedUnit.Summon();

        if (isFirst)
            unit.FirstTurnDiscountUndo();

        BattleUI.RemoveHandUnit(unit);
        GameManager.UI.ClosePopup();
        Field.ClearAllColor();
    }

    public void OnClickTile(Tile tile)
    {
        coord = Field.FindCoordByTile(tile);
        _phase.OnClickEvent();
    }
    #endregion

    public void UnitSetting(BattleUnit unit, Vector2 coord, Team team)
    {
        unit.SetTeam(team);
        Field.EnterTile(unit, coord);
        unit.UnitDeadAction = UnitDeadAction;

        Data.BattleUnitAdd(unit);
        //Data.BattleUnitOrder();
    }

    public IEnumerator UnitAttack()
    {
        UnitAttackAction();

        yield return StartCoroutine(BattleCutScene.AfterAttack());
        yield return new WaitUntil(() => Data.CorruptUnits.Count == 0);

        EndUnitAction();
    }

    public void AttackStart(BattleUnit caster, BattleUnit hit)
    {
        List<BattleUnit> hits = new();
        hits.Add(hit);

        Data.HitUnits = hits;
        BattleCutScene.BattleCutScene(caster, Data.HitUnits);
    }
    public void AttackStart(BattleUnit caster, List<BattleUnit> hits)
    {
        Data.HitUnits = hits;
        BattleCutScene.BattleCutScene(caster, Data.HitUnits);
    }

    // 애니메이션용 추가
    private void UnitAttackAction()
    {
        BattleUnit unit = Data.GetNowUnit();
        
        foreach (BattleUnit hit in Data.HitUnits)
        {
            if (hit == null)
                continue;
            Team team = hit.Team;

            //공격 전 낙인 체크
            //unit.PassiveCheck(unit, hit, ActiveTiming.BEFORE_ATTACK);
            unit.SkillUse(hit);

            if (unit.SkillEffectAnim != null)
                GameManager.VisualEffect.StartVisualEffect(unit.SkillEffectAnim, hit.transform.position);

            if (hit.HP.GetCurrentHP() <= 0)
                continue;

            //unit.PassiveCheck(unit, hit, ActiveTiming.AFTER_ATTACK);
        }

        string unitname = unit.DeckUnit.Data.Name;
        string faction = unit.DeckUnit.Data.Faction.ToString();
        Debug.Log(unitname + "   " + faction);
        GameManager.Sound.Play("Character/" + faction + "/" + unitname + "/" + unitname + "_Attack");

    }

    public void EndUnitAction()
    {
        Field.ClearAllColor();
        Data.BattleOrderRemove(Data.GetNowUnit());
        BattleOverCheck();
        _phase.ChangePhase(_phase.Engage);
        BattleUI.UI_darkEssence.Refresh();
    }

    public void StigmaSelectEvent(Corruption cor)
    {
        BattleUnit targetUnit = cor.GetTargetUnit();

        if (targetUnit.Team == Team.Enemy)
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(targetUnit.DeckUnit, 2, cor.LoopExit);
        else
            cor.LoopExit();
    }

    private void UnitDeadAction(BattleUnit unit)
    {
        GameManager.VisualEffect.StartVisualEffect(Resources.Load<AnimationClip>("Animation/UnitDeadEffect"), unit.transform.position);

        StartCoroutine(UnitDeadEffect(unit));
    }

    private IEnumerator UnitDeadEffect(BattleUnit unit)
    {
        Data.BattleUnitRemove(unit);
        Data.BattleOrderRemove(unit);

        SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();

        while (true)
        {
            Color c = sr.color;
            float a = c.a - 0.01f;
            c.a = a;

            sr.color = c;

            if (c.a <= 0)
                break;

            yield return null;
        }
        Destroy(unit.gameObject);
    }

    public void DirectAttack()
    {
        //핸드에 있는 유닛을 하나 무작위로 제거하고 배틀 종료 체크
        Debug.Log("Direct Attack");

        if (Data.PlayerHands.Count == 0)
        {
            BattleOverCheck();
            return;
        }

        int randNum = UnityEngine.Random.Range(0, Data.PlayerHands.Count);
        BattleUI.RemoveHandUnit(Data.PlayerHands[randNum]);

        BattleOverCheck();
    }

    public void BattleOverCheck()
    {
        int MyUnit = 0;
        int EnemyUnit = 0;

        foreach (BattleUnit BUnit in Data.BattleUnitList)
        {
            if (BUnit.Team == Team.Player)//아군이면
                MyUnit++;
            else
                EnemyUnit++;
        }

        MyUnit += Data.PlayerDeck.Count;
        MyUnit += Data.PlayerHands.Count;
        //EnemyUnit 대기 중인 리스트만큼 추가하기

        if (MyUnit == 0)
        {
            BattleOverLose();
        }
        else if (EnemyUnit == 0)
        {
            BattleOverWin();
        }
    }

    private void BattleOverWin()
    {
        Debug.Log("YOU WIN");
        Data.OnBattleOver();
        _phase.ChangePhase(new BattleOverPhase());
        if(GameManager.Data.CurrentStageData.Level == 11)
            GameManager.UI.ShowScene<UI_BattleOver>().SetImage("elite win");
        else
            GameManager.UI.ShowScene<UI_BattleOver>().SetImage("win");
    }

    private void BattleOverLose()
    {
        Debug.Log("YOU LOSE");
        _phase.ChangePhase(new BattleOverPhase());
        GameManager.UI.ShowScene<UI_BattleOver>().SetImage("lose");
        GameManager.Data.DeckClear();
    }

    // 이동 경로를 받아와 이동시킨다
    private void MoveLocate(BattleUnit caster, Vector2 coord)
    {
        Vector2 current = caster.Location;
        Vector2 dest = current + coord;

        Field.MoveUnit(current, dest);
        GameManager.Sound.Play("Move/MoveSFX");
    }

    public List<BattleUnit> GetArroundUnits(List<Vector2> coords)
    {
        List<BattleUnit> units = new List<BattleUnit>();

        foreach (Vector2 coord in coords)
        {
            BattleUnit targetUnit = Field.GetUnit(coord);
            if (targetUnit == null)
                continue;
            units.Add(targetUnit);
        }

        return units;
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));

    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }

    #region Field Color 관련
    public bool UnitSpawnReady(FieldColorType colorType)
    {
        if (_phase.Current != _phase.Prepare)
            return false;

        if (colorType == FieldColorType.none)
            Field.ClearAllColor();
        else
            Field.SetSpawnTileColor();
        
        fieldColorType = colorType;

        return true;
    }
    #endregion
}