using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class UnitAction_Yohrn : UnitAction
{
    const float _moveSpeed = 1.2f;
    private bool _firstMoveCheck = false;
    private int _isEvenTileAttackTurn = 0;//0�̸� Ȧ��, 1�̸� ¦�� ĭ�� ����
    readonly private List<Vector2> _upDownCoord = new() { Vector2.down, Vector2.down * 2, Vector2.zero, Vector2.up, Vector2.up * 2 };

    private IEnumerator _moveCoroutine;

    private List<BattleUnit> _subUnitList = new();
    private Dictionary<BattleUnit, UnitAction_Yohrn_Scale> _subUnitActionDict = new();
    private Dictionary<Vector2, GameObject> _portalDict = new();

    private Vector2 _moveDirectionVector;
    private Vector2 _moveOppositeVector;

    private bool _isFall = false;
    private bool _actionBlock = false;
    private bool _isBattleMove = false;

    public override void AIMove(BattleUnit attackUnit)
    {
        _isBattleMove = true;
        UnitMoveAction(attackUnit);
    }

    public override void AISkillUse(BattleUnit attackUnit)
    {
        if (_actionBlock)
        {
            BattleManager.Instance.PlayAfterCoroutine(() => {
                AISkillUse(attackUnit);
            }, 0.1f);
            return;
        }

        List<BattleUnit> targetUnits = new();
        List<BattleUnit> attackSubUnits = new();

        foreach (BattleUnit subUnit in _subUnitList)
        {
            if (subUnit.Buff.CheckBuff(BuffEnum.Scale))
            {
                attackSubUnits.Add(subUnit);
                targetUnits.AddRange(BattleManager.Field.GetUnitsInRange(subUnit.Location, _upDownCoord,
                    (attackUnit.Team == Team.Enemy) ? Team.Player : Team.Enemy));
            }
        }

        if (targetUnits.Count > 0)
        {
            _actionBlock = true;
            foreach (BattleUnit subUnit in _subUnitList)
            {
                if (attackSubUnits.Contains(subUnit))
                {
                    subUnit.AnimatorSetBool("isAttack", true);
                    GameManager.VisualEffect.StartVisualEffect("Arts/EffectAnimation/AttackEffect/Yohrn_Attack",
                        BattleManager.Field.GetTilePosition(subUnit.Location) + new Vector3(0f, 1f, 0f));
                }

                subUnit.UnitRenderer.sortingOrder = 5;
            }

            BattleManager.Instance.AttackStart(attackUnit, targetUnits.Distinct().ToList(), true);
        }
        else
        {
            _actionBlock = false;
            BattleManager.Instance.EndUnitAction();
        }
    }

    private void UnitMoveAction(BattleUnit attackUnit)
    {
        if (_actionBlock)
        {
            BattleManager.Instance.PlayAfterCoroutine(() => {
                UnitMoveAction(attackUnit);
            }, 0.1f);
            return;
        }

        Vector2 moveVector = attackUnit.Location + _moveDirectionVector;

        if (BattleManager.Field.IsInRange(moveVector))
        {
            BattleUnit frontUnit = BattleManager.Field.TileDict[moveVector].Unit;
            if (frontUnit != null)
            {
                _field.ExitTile(frontUnit.Location);
                frontUnit.UnitDiedEvent(false);
                attackUnit.AnimatorSetBool("isMoveAttack", true);
                GameManager.Sound.Play("Character/��/��_MoveAttack");
                BattleManager.Instance.PlayAfterCoroutine(() => {
                    attackUnit.AnimatorSetBool("isMoveAttack", false);
                }, 1f);
                //���̱�
            }

            _field.ExitTile(attackUnit.Location);
            _field.EnterTile(attackUnit, moveVector);

            if (_moveCoroutine != null)
                BattleManager.Instance.StopCoroutine(_moveCoroutine);
            _moveCoroutine = MoveFieldPosition(attackUnit, BattleManager.Field.GetTilePosition(moveVector), moveVector);
            BattleManager.Instance.StartCoroutine(_moveCoroutine);

            foreach (BattleUnit subUnit in _subUnitList)
            {
                _subUnitActionDict[subUnit].UnitMoveAction(false);
            }

            SetMoveTileColor(attackUnit);
            SubUnitSpawn(attackUnit);

            if (!_isBattleMove)
                return;

            if (!_firstMoveCheck)
            {
                //�� �� �� ��������� ����
                _firstMoveCheck = true;
                BattleManager.Instance.PlayAfterCoroutine(() => {
                    UnitMoveAction(attackUnit);
                }, 3f);
            }
            else
            {
                _firstMoveCheck = false;
                BattleManager.Instance.PlayAfterCoroutine(() => {
                    BattleManager.Phase.ChangePhase(BattleManager.Phase.Action);
                    BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(true);
                }, 3f);
            }
        }
        else
        {
            UnitChangeRow(attackUnit, false);
        }
    }

    private void UnitChangeRow(BattleUnit attackUnit, bool isBackMove)
    {
        if (_actionBlock)
        {
            BattleManager.Instance.PlayAfterCoroutine(() => {
                UnitChangeRow(attackUnit, isBackMove);
            }, 0.1f);
            return;
        }

        int moveX = (attackUnit.Team == Team.Enemy) == isBackMove ? 0 : 5;
        int moveY = attackUnit.Location.y switch
        {
            0 => isBackMove ? 2 : 1,
            1 => 0,
            2 => isBackMove ? -1 : 0,
            _ => -1
        };

        Vector2 moveVector = new(moveX, moveY);
        if (moveVector.y == -1)
            return;

        _actionBlock = true;

        BattleUnit frontUnit = BattleManager.Field.TileDict[moveVector].Unit;
        if (frontUnit != null)
        {
            BattleManager.Field.ExitTile(frontUnit.Location);
            BattleManager.Instance.PlayAfterCoroutine(() => {
                frontUnit.UnitDiedEvent(false);
                attackUnit.AnimatorSetBool("isMoveAttack", true);
                BattleManager.Instance.PlayAfterCoroutine(() => {
                    attackUnit.AnimatorSetBool("isMoveAttack", false);
                }, 2f);
            }, 3f);
            //���̱�
        }

        if (!_portalDict.ContainsKey(attackUnit.Location))
            CreatePortal(attackUnit.Location);

        BattleManager.Field.ExitTile(attackUnit.Location);
        BattleManager.Field.EnterTile(attackUnit, moveVector);
        
        BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimation(attackUnit, moveVector));

        foreach (BattleUnit subUnit in _subUnitList)
        {
            _subUnitActionDict[subUnit].UnitMoveAction(false);
        }
        SubUnitSpawn(attackUnit);

        if (!_firstMoveCheck)
        {
            //�� �� �� ��������� ����
            _firstMoveCheck = true;
            BattleManager.Instance.PlayAfterCoroutine(() => {
                UnitMoveAction(attackUnit);
            }, 2f);
        }
        else
        {
            _firstMoveCheck = false;
            BattleManager.Instance.PlayAfterCoroutine(() => {
                BattleManager.Phase.ChangePhase(BattleManager.Phase.Action);
                BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(true);
            }, 2f);
        }
    }

    public BattleUnit SubUnitSpawn(BattleUnit caster)
    {
        SpawnData sd = new();
        sd.unitData = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/��_��ü");
        sd.location = (caster.Team == Team.Enemy) ? new(5, 2) : new(0, 2);
        sd.team = caster.Team;

        BattleUnit spawnUnit = BattleManager.Spawner.SpawnDataSpawn(sd);

        if (spawnUnit == null)
            return null;

        _subUnitList.Add(spawnUnit);
        _subUnitList = _subUnitList
            .OrderBy(unit =>
                unit.Location.y == 1 ? 0 :
                unit.Location.y == 0 ? 1 :
                unit.Location.y == 2 ? 2 : 3)  // y���� Ư�� ������ ����
            .ThenBy(unit => (unit.Team == Team.Enemy) ? unit.Location.x : -unit.Location.x)
            .ToList();

        UnitAction_Yohrn_Scale spawnUnitAction = new();
        spawnUnit.Action = spawnUnitAction;
        _subUnitActionDict.Add(spawnUnit, spawnUnitAction);
        spawnUnitAction.Init(caster, this, spawnUnit);

        BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimationCoroutine(spawnUnit, false));

        return spawnUnit;
    }

    private void UnitBackMove(BattleUnit unit)
    {
        if (_actionBlock)
        {
            BattleManager.Instance.PlayAfterCoroutine(() => {
                UnitBackMove(unit);
            }, 0.1f);
            return;
        }

        _actionBlock = true;
        for (int i = _subUnitList.Count - 1; i >= 0; i--)
        {
            _subUnitActionDict[_subUnitList[i]].UnitMoveAction(true);
        }

        if (BattleManager.Field.IsInRange(unit.Location + _moveOppositeVector))
        {
            _field.ExitTile(unit.Location);
            _field.EnterTile(unit, unit.Location + _moveOppositeVector);

            if (_moveCoroutine != null)
                BattleManager.Instance.StopCoroutine(_moveCoroutine);
            _moveCoroutine = MoveFieldPosition(unit, BattleManager.Field.GetTilePosition(unit.Location + _moveOppositeVector), unit.Location + _moveOppositeVector);
            BattleManager.Instance.StartCoroutine(_moveCoroutine);
        }
        else
        {
            UnitChangeRow(unit, true);
        }
    }

    private void SetMoveTileColor(BattleUnit unit)
    {
        Tile tile = BattleManager.Field.TileDict[unit.Location];
        tile.IsColored = true;
        tile.SetColor(BattleManager.Field.ColorList(FieldColorType.Move));

        foreach (BattleUnit subUnit in _subUnitList)
        {
            tile = BattleManager.Field.TileDict[subUnit.Location];
            tile.IsColored = true;
            tile.SetColor(BattleManager.Field.ColorList(FieldColorType.Move));
        }
    }

    private void CreatePortal(Vector2 location)
    {
        Vector3 portalTransformPosition = BattleManager.Field.GetTilePosition(location);
        portalTransformPosition.x += location.x == 0 ? -1.8f : 1.8f;
        portalTransformPosition.y += 1.5f;

        GameObject portal = GameManager.Resource.Instantiate("BattleUnits/Yohrn_Portal");
        portal.transform.position = portalTransformPosition;

        if (location.x == 0)
            portal.transform.rotation = new(0f, 180f, 0f, 0f);

        _portalDict.Add(location, portal);
    }

    private void SetAttackUnitBuff()
    {
        foreach (BattleUnit subUnit in _subUnitList)
        {
            if (subUnit.Buff.CheckBuff(BuffEnum.Scale))
                subUnit.DeleteBuff(BuffEnum.Scale);

            if (subUnit.Location.x % 2 == _isEvenTileAttackTurn)
            {
                subUnit.SetBuff(new Buff_Scale());
            }
        }

        _isEvenTileAttackTurn = (_isEvenTileAttackTurn + 1) % 2;
    }

    public IEnumerator UnitPortalMoveAnimation(BattleUnit unit, Vector2 moverLocation)
    {
        yield return BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimationCoroutine(unit, true));
        unit.SetLocation(moverLocation);

        if (!_portalDict.ContainsKey(unit.Location))
            CreatePortal(unit.Location);

        yield return BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimationCoroutine(unit, false));
        unit.SetLocation(moverLocation);

        _actionBlock = false;
    }

    public IEnumerator UnitFirstAppearAnimation(BattleUnit unit)
    {
        BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(false);
        yield return BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimationCoroutine(unit, false));

        UnitMoveAction(unit);
        yield return new WaitForSeconds(2.7f);
        UnitMoveAction(unit);
        yield return new WaitForSeconds(2.7f);

        BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(true);
        _actionBlock = false;
        SetAttackUnitBuff();
    }

    public IEnumerator UnitPortalMoveAnimationCoroutine(BattleUnit unit, bool isPortalEnter)
    {
        Vector3 moveDirection = new(unit.Location.x == 0 ? -4 : 4, 0, 0);

        Vector3 moveStartPosition = unit.transform.position + (isPortalEnter ? Vector3.zero : moveDirection);
        Vector3 moveEndPosition = unit.transform.position + (isPortalEnter ? moveDirection : Vector3.zero);

        unit.transform.position = moveStartPosition;
        unit.UnitRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        yield return BattleManager.Instance.StartCoroutine(MoveFieldPositionCoroutine(unit, moveEndPosition, _moveSpeed * 1.5f));

        unit.UnitRenderer.maskInteraction = SpriteMaskInteraction.None;
        unit.SetLocation(unit.Location);
    }

    public override bool ActionTimingCheck(ActiveTiming activeTiming, BattleUnit caster, BattleUnit receiver)
    {
        if ((activeTiming & ActiveTiming.SUMMON) == ActiveTiming.SUMMON)
        {
            Vector2 newLocation = (caster.Team == Team.Enemy) ? new Vector2(5, 2) : new Vector2(0, 2);
            _moveDirectionVector = (caster.Team == Team.Enemy) ? Vector2.left : Vector2.right;
            _moveOppositeVector = (caster.Team == Team.Enemy) ? Vector2.right : Vector2.left;

            BattleManager.Field.ExitTile(caster.Location);
            BattleManager.Field.EnterTile(caster, newLocation);
            caster.SetLocation(newLocation);

            BattleUnit frontUnit = BattleManager.Field.TileDict[caster.Location].Unit;
            if (frontUnit != null && frontUnit != caster)
            {
                frontUnit.UnitDiedEvent(false);
                caster.AnimatorSetBool("isMoveAttack", true);
                BattleManager.Instance.PlayAfterCoroutine(() => {
                    caster.AnimatorSetBool("isMoveAttack", false);
                }, 2f);
                //���̱�
            }

            CreatePortal(caster.Location);
            if (caster.Team == Team.Enemy)
            {
                BattleManager.Instance.StartCoroutine(UnitFirstAppearAnimation(caster));
            }
            else
            {
                BattleManager.Instance.StartCoroutine(UnitPortalMoveAnimationCoroutine(caster, false));
            }
        }
        else if ((activeTiming & ActiveTiming.BEFORE_CHANGE_FALL) == ActiveTiming.BEFORE_CHANGE_FALL)
        {
            return receiver != caster;
        }
        else if ((activeTiming & ActiveTiming.TURN_START) == ActiveTiming.TURN_START)
        {
            SetAttackUnitBuff();
        }
        else if ((activeTiming & ActiveTiming.ATTACK_TURN_START) == ActiveTiming.ATTACK_TURN_START)
        {
            foreach (BattleUnit subUnit in _subUnitList)
            {
                Tile tile = BattleManager.Field.TileDict[subUnit.Location];
                tile.IsColored = true;
                tile.SetColor(BattleManager.Field.ColorList(FieldColorType.Attack));

                if (!subUnit.Buff.CheckBuff(BuffEnum.Scale))
                    continue;

                foreach (Vector2 vec in _upDownCoord)
                {
                    Vector2 targetLocation = vec + subUnit.Location;

                    if (BattleManager.Field.IsInRange(targetLocation))
                    {
                        tile = BattleManager.Field.TileDict[targetLocation];
                        tile.IsColored = true;
                        tile.SetColor(BattleManager.Field.ColorList(FieldColorType.Attack));
                    }
                }
            }
        }
        else if ((activeTiming & ActiveTiming.ATTACK_TURN_END) == ActiveTiming.ATTACK_TURN_END)
        {
            _actionBlock = false;
        }
        else if ((activeTiming & ActiveTiming.MOVE_TURN_START) == ActiveTiming.MOVE_TURN_START)
        {
            if (caster.Team == Team.Player)
            {
                UnitMoveAction(caster);
                BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(false);
            }

            SetMoveTileColor(caster);
        }
        else if ((activeTiming & ActiveTiming.FIELD_UNIT_DEAD) == ActiveTiming.FIELD_UNIT_DEAD)
        {
            if (!_subUnitList.Contains(receiver))
                return false;

            _subUnitList.Remove(receiver);
            if (receiver.Team == caster.Team)
            {
                caster.GetAttack(-receiver.BattleUnitTotalStat.MaxHP, null);
                UnitBackMove(caster);
            }
        }
        else if ((activeTiming & ActiveTiming.FIELD_UNIT_FALLED) == ActiveTiming.FIELD_UNIT_FALLED)
        {
            if (!_subUnitList.Contains(receiver))
                return false;

            _subUnitList.Remove(receiver);
            if (receiver.Team == caster.Team)
            {
                receiver.UnitDiedEvent(false);
                caster.ChangeFall(1, caster, FallAnimMode.On);
                UnitBackMove(caster);
            }
        }
        else if ((activeTiming & ActiveTiming.AFTER_UNIT_DEAD) == ActiveTiming.AFTER_UNIT_DEAD)
        {
            if (BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "��" && findUnit != caster) != null)
            {
                while (true)
                {
                    BattleUnit remainUnit = BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "��_��ü" && findUnit.Team == caster.Team);
                    if (remainUnit == null)
                        break;

                    remainUnit.UnitDiedEvent(false);
                }
            }
        }
        else if ((activeTiming & ActiveTiming.FALLED) == ActiveTiming.FALLED)
        {
            if (!_isFall && caster.Team == Team.Enemy)
            {
                BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(false);
                BattleManager.Instance.SetTlieClickCoolDown(4f);

                caster.AnimatorSetBool("isCorrupt", true);
                BattleManager.Instance.PlayAfterCoroutine(() =>
                {
                    caster.UnitFallEvent();
                }, 2f);

                _isFall = true;

                return true;
            }
            else
            {
                if (BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "��" && findUnit != caster) != null)
                {
                    while (true)
                    {
                        BattleUnit remainUnit = BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "��_��ü" && findUnit.Team == caster.Team);
                        if (remainUnit == null)
                            break;

                        remainUnit.UnitFallEvent();
                    }
                }

                _isFall = false;

                return false;
            }
        }
        else if ((activeTiming & ActiveTiming.BEFORE_ATTACK) == ActiveTiming.BEFORE_ATTACK)
        {
            if (receiver != null)
            {
                receiver.ChangeFall(1, caster, FallAnimMode.On);
            }
        }
        else if ((activeTiming & ActiveTiming.ATTACK_MOTION_END) == ActiveTiming.ATTACK_MOTION_END)
        {
            foreach (BattleUnit subUnit in _subUnitList)
            {
                subUnit.AnimatorSetBool("isAttack", false);
                subUnit.UnitRenderer.sortingOrder = 2;
            }
        }

        return false;
    }

    public override bool ActionStart(BattleUnit attackUnit, List<BattleUnit> hits, Vector2 coord)
    {
        if (!BattleManager.Field.TileDict[coord].IsColored || 
            BattleManager.Field.GetUnit(coord) == null || 
            BattleManager.Field.GetUnit(coord).Team == attackUnit.Team ||
            _actionBlock)
            return false;

        List<BattleUnit> targetUnits = new();
        List<BattleUnit> attackSubUnits = new();

        foreach (BattleUnit subUnit in _subUnitList)
        {
            if (subUnit.Buff.CheckBuff(BuffEnum.Scale))
            {
                attackSubUnits.Add(subUnit);
                targetUnits.AddRange(BattleManager.Field.GetUnitsInRange(subUnit.Location, _upDownCoord,
                    (attackUnit.Team == Team.Enemy) ? Team.Player : Team.Enemy));
            }
        }

        if (targetUnits.Count > 0)
        {
            _actionBlock = true;
            foreach (BattleUnit subUnit in _subUnitList)
            {
                if (attackSubUnits.Contains(subUnit))
                {
                    subUnit.AnimatorSetBool("isAttack", true);
                    GameManager.VisualEffect.StartVisualEffect("Arts/EffectAnimation/AttackEffect/Yohrn_Attack",
                        BattleManager.Field.GetTilePosition(subUnit.Location) + new Vector3(0f, 1f, 0f));
                }

                subUnit.UnitRenderer.sortingOrder = 5;
            }

            BattleManager.Instance.AttackStart(attackUnit, targetUnits.Distinct().ToList(), true);
        }
        else
        {
            _actionBlock = false;
            BattleManager.Instance.EndUnitAction();
        }

        return true;
    }

    public override List<Vector2> GetSplashRangeForField(BattleUnit unit, Tile targetTile, Vector2 caster)
    {
        List<Vector2> splashRangeList = new();
        foreach (BattleUnit subUnit in _subUnitList)
        {
            if (subUnit.Location.x % 2 != _isEvenTileAttackTurn)
                continue;

            foreach (Vector2 vec in _upDownCoord)
            {
                if (BattleManager.Field.IsInRange(subUnit.Location + vec))
                {
                    BattleUnit locationUnit = BattleManager.Field.TileDict[subUnit.Location + vec].Unit;
                    if (_subUnitList.Contains(locationUnit) || locationUnit == unit)
                        continue;

                    splashRangeList.Add(subUnit.Location + vec);
                }
            }
        }

        return splashRangeList;
    }

    public IEnumerator MoveFieldPosition(BattleUnit unit, Vector3 moveDestination, Vector2 coord)
    {
        _moveCoroutine = MoveFieldPositionCoroutine(unit, moveDestination, _moveSpeed);
        yield return BattleManager.Instance.StartCoroutine(_moveCoroutine);
        unit.SetLocation(coord);
        _actionBlock = false;
        BattleManager.Instance.ActiveTimingCheck(ActiveTiming.MOVE, unit);
    }

    public IEnumerator MoveFieldPositionCoroutine(BattleUnit unit, Vector3 moveDestination, float moveSpeed)
    {
        //������ ���� ���ɼ�(������ �� �ٲ�� Ȯ���غ���)
        float addScale = unit.transform.localScale.x;

        unit.SetFlipX(_moveDirectionVector == Vector2.left);

        Vector3 pVel = Vector3.zero;
        Vector3 sVel = Vector3.zero;

        float moveTime = 1f / moveSpeed;

        while (Vector3.Distance(moveDestination, unit.transform.position) >= 0.03f)
        {
            unit.transform.position = Vector3.SmoothDamp(unit.transform.position, moveDestination, ref pVel, moveTime);
            unit.transform.localScale = Vector3.SmoothDamp(unit.transform.localScale, new(addScale, addScale, 1), ref sVel, moveTime);

            yield return null;
        }

        pVel = Vector3.zero;
    }
}
