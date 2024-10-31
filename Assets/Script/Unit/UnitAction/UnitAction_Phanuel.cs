using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitAction_Phanuel : UnitAction
{
    private int[] _recentState = {-1, -1};

    private int _phanuelState = 0;
    private List<Vector2> _attackTile = new();
    private Phanuel_Animation _phanuel_Animation = null;
    private bool _isFall = false;

    public override void AIMove(BattleUnit attackUnit)
    {
        BattleManager.Phase.ChangePhase(BattleManager.Phase.Action);
    }
     
    public override void AISkillUse(BattleUnit attackUnit)
    {
        if (DirectAttackCheck())
        {
            BattleManager.Instance.DirectAttack(attackUnit);
        }

        List<BattleUnit> targetUnits = new();
        List<Vector2> emptyTiles = new();

        foreach (Vector2 tile in  _attackTile)
        {
            BattleUnit unitOnTile = BattleManager.Field.GetUnit(tile);

            if (unitOnTile != null && unitOnTile.Team != attackUnit.Team)
            {
                targetUnits.Add(unitOnTile);
            }
            else if (unitOnTile == null)
            {
                emptyTiles.Add(tile);
            }
        }

        if (emptyTiles.Count > 0)
        {
            SpawnData sd = new();
            sd.unitData = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/오벨리스크");
            sd.location = emptyTiles[Random.Range(0, emptyTiles.Count)];
            sd.team = attackUnit.Team;

            BattleManager.Spawner.SpawnDataSpawn(sd);
        }

        if (targetUnits.Count > 0)
        {
            _phanuel_Animation.SetBool("isAttack", true);
            BattleManager.Instance.AttackStart(attackUnit, targetUnits.Distinct().ToList());
        }
        else if (!DirectAttackCheck())
        {
            BattleManager.Instance.EndUnitAction();
        }

        TileClear(attackUnit.Team);
    }

    private void PhanuelStateCheck(BattleUnit caster)
    {
        if (_recentState[0] == -1)
        {
            _phanuelState = 0;
        }
        else 
        {
            int count = 0;

            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Data.ID == "오벨리스크" && unit.Team == caster.Team)
                {
                    count++;
                }
            }

            if (count >= 6)
            {
                _phanuelState = 2;
            }
            else if (count == 0)
            {
                _phanuelState = 0;
            }
            else if (_recentState[0] == _recentState[1])
            {
                _phanuelState = _recentState[0] == 0 ? 1 : 0;
            }
            else
            {
                _phanuelState = Random.Range(0, 2);
            }
        }

        foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
        {
            if (unit.Data.ID == "오벨리스크" && unit.Team == caster.Team)
            {
                unit.AnimatorSetBool("isBright", _phanuelState == 2);
            }
        }

        _recentState[0] = _phanuelState;
        _recentState[1] = _recentState[0];
    }

    private void SetAttackTile(BattleUnit caster)
    {
        TileClear(caster.Team);
        if (_phanuelState == 0)
        {
            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Team != caster.Team)
                {
                    TileAdd(unit.Location);
                }
            }
        }
        else if (_phanuelState == 1)
        {
            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Data.ID == "오벨리스크" && unit.Team == caster.Team)
                {
                    List<Vector2> UDLR = new() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

                    foreach (Vector2 udrl in UDLR)
                    {
                        if (!BattleManager.Field.IsInRange(unit.Location + udrl))
                            continue;

                        BattleUnit udrlUnit = BattleManager.Field.GetUnit(unit.Location + udrl);
                        if (udrlUnit != null && (udrlUnit.Data.ID == "오벨리스크" && udrlUnit.Team == caster.Team) || udrlUnit == caster)
                        {
                            continue;
                        }

                        TileAdd(unit.Location + udrl);
                    }
                }
            }
        }
        else if (_phanuelState == 2)
        {
            List<Vector2> nonAttackTiles = new();

            foreach (ConnectedUnit unit in caster.ConnectedUnits)
            {
                nonAttackTiles.Add(unit.Location);
            }
            nonAttackTiles.Add(caster.Location);

            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if ((unit.Data.ID == "오벨리스크" && unit.Team == caster.Team))
                {
                    nonAttackTiles.Add(unit.Location);
                }
            }

            foreach (Vector2 tile in BattleManager.Field.TileDict.Keys)
            {
                if (!nonAttackTiles.Contains(tile))
                {
                    TileAdd(tile);
                }
            }
        }

        if (caster.Team == Team.Enemy)
        {
            BattleManager.Field.SetEffectTile(_attackTile, EffectTileType.Phanuel_Attack_Enemy);
        }
        else if (caster.Team == Team.Player)
        {
            BattleManager.Field.SetEffectTile(_attackTile, EffectTileType.Phanuel_Attack_Friendly);
        }
    }

    private void TileAdd(Vector2 coord)
    {
        _attackTile.Add(coord);
    }

    private void TileClear(Team team)
    {
        _attackTile.Clear();

        if (team == Team.Enemy)
            BattleManager.Field.ClearEffectTile(_attackTile, EffectTileType.Phanuel_Attack_Enemy);
        else if (team == Team.Player)
            BattleManager.Field.ClearEffectTile(_attackTile, EffectTileType.Phanuel_Attack_Friendly);
    }

    public override bool ActionTimingCheck(ActiveTiming activeTiming, BattleUnit caster, BattleUnit receiver) 
    {
        if ((activeTiming & ActiveTiming.SUMMON) == ActiveTiming.SUMMON || (activeTiming & ActiveTiming.STIGMA) == ActiveTiming.STIGMA)
        {
            if (_phanuel_Animation == null)
            {
                GameManager.Sound.Play("PhanuelSummon/Phanuel_Summon");
                _phanuel_Animation = GameManager.Resource.Instantiate("BattleUnits/Phanuel_Animation", caster.transform).GetComponent<Phanuel_Animation>();
                _phanuel_Animation.SetAnimator(caster.Team);
            }
            else
            {
                _phanuel_Animation.ChangeAnimator(caster.Team);
            }
        }
        else if ((activeTiming & ActiveTiming.TURN_START) == ActiveTiming.TURN_START)
        {
            PhanuelStateCheck(caster);
            SetAttackTile(caster);
        }
        else if ((activeTiming & ActiveTiming.FIELD_UNIT_SUMMON) == ActiveTiming.FIELD_UNIT_SUMMON)
        {
            if (_phanuelState == 0)
            {
                SetAttackTile(caster);
            }
        }
        else if ((activeTiming & ActiveTiming.FIELD_UNIT_DEAD) == ActiveTiming.FIELD_UNIT_DEAD)
        {
            if (_phanuelState == 1)
            {
                SetAttackTile(caster);
            }
        }
        else if ((activeTiming & ActiveTiming.AFTER_UNIT_DEAD) == ActiveTiming.AFTER_UNIT_DEAD)
        {
            if (BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "바누엘" && findUnit != caster) != null)
            {
                while (true)
                {
                    BattleUnit remainUnit = BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "오벨리스크" && findUnit.Team == caster.Team);
                    if (remainUnit == null)
                        break;

                    remainUnit.UnitDiedEvent(false);
                }

                TileClear(caster.Team);
            }
        }
        else if ((activeTiming & ActiveTiming.FALLED) == ActiveTiming.FALLED)
        {
            if (!_isFall && caster.Team == Team.Enemy)
            {
                BattleManager.BattleUI.UI_TurnChangeButton.SetEnable(false);
                BattleManager.Instance.SetTlieClickCoolDown(4f);

                _phanuel_Animation.SetBool("isCorrupt", true);
                GameManager.Instance.PlayAfterCoroutine(() =>
                {
                    caster.UnitFallEvent();
                }, 2f);

                _isFall = true;

                return true;
            }
            else
            {
                if (BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "바누엘" && findUnit != caster) != null)
                {
                    while (true)
                    {
                        BattleUnit remainUnit = BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "오벨리스크" && findUnit.Team == caster.Team);
                        if (remainUnit == null)
                            break;

                        remainUnit.UnitDiedEvent(false);
                    }

                    TileClear(caster.Team);
                }

                _isFall = false;

                return false;
            }
        }
        else if ((activeTiming & ActiveTiming.ATTACK_TURN_START) == ActiveTiming.ATTACK_TURN_START)
        {
            foreach (Vector2 tile in _attackTile)
            {
                if (BattleManager.Field.IsInRange(tile))
                {
                    BattleManager.Field.TileDict[tile].IsColored = true;
                    BattleManager.Field.TileDict[tile].SetColor(BattleManager.Field.ColorList(FieldColorType.Attack));
                }
            }
        }
        else if ((activeTiming & ActiveTiming.ATTACK_TURN_END) == ActiveTiming.ATTACK_TURN_END)
        {
            _phanuel_Animation.SetBool("isAttack", false);
        }
        else if ((activeTiming & ActiveTiming.BEFORE_ATTACK) == ActiveTiming.BEFORE_ATTACK)
        {
            if (receiver != null)
            {
                receiver.ChangeFall(1, caster, FallAnimMode.On);
            }
        }

        return false;
    }

    public override bool ActionStart(BattleUnit attackUnit, List<BattleUnit> hits, Vector2 coord)
    {
        if (!_attackTile.Contains(coord))
            return false;

        List<BattleUnit> targetUnits = new();
        List<Vector2> emptyTiles = new();

        foreach (Vector2 tile in _attackTile)
        {
            BattleUnit unitOnTile = BattleManager.Field.GetUnit(tile);

            if (unitOnTile != null && unitOnTile.Team != attackUnit.Team)
            {
                targetUnits.Add(unitOnTile);
            }
            else if (unitOnTile == null)
            {
                emptyTiles.Add(tile);
            }
        }

        SpawnData sd = new();
        sd.unitData = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/오벨리스크");
        sd.team = attackUnit.Team;

        if (hits.Count == 0)
        {
            sd.location = coord;
            BattleManager.Spawner.SpawnDataSpawn(sd);
        }
        else if (emptyTiles.Count > 0)
        {
            sd.location = emptyTiles[Random.Range(0, emptyTiles.Count)];
            BattleManager.Spawner.SpawnDataSpawn(sd);
        }

        if (targetUnits.Count > 0)
        {
            _phanuel_Animation.SetBool("isAttack", true);
            BattleManager.Instance.AttackStart(attackUnit, targetUnits.Distinct().ToList());
        }
        else
        {
            BattleManager.Instance.EndUnitAction();
        }

        TileClear(attackUnit.Team);

        return true;
    }

    public override List<Vector2> GetSplashRangeForField(BattleUnit unit, Tile targetTile, Vector2 caster)
    {
        return _attackTile;
    }
}