using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAction_Arabella : UnitAction
{
    readonly List<Vector2> UDLR = new() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

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

        Dictionary<Vector2, int> hpList = new();
        foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
        {
            if (unit.Team == attackUnit.Team)
                continue;

            foreach (Vector2 direction in UDLR)
            {
                Vector2 location = unit.Location + direction;
                if (BattleManager.Field.IsInRange(location) && !BattleManager.Field.TileDict[location].UnitExist)
                {
                    if (!hpList.ContainsKey(location))
                    {
                        hpList.Add(location, unit.GetHP());
                    }
                    else if (hpList[location] > unit.GetHP())
                    {
                        hpList[location] = unit.GetHP();
                    }
                }
            }
        }

        List<Vector2> MinHPUnit = MinHPSearch(hpList);

        if (MinHPUnit.Count > 0)
        {
            Vector2 moveLocation = MinHPUnit[Random.Range(0, MinHPUnit.Count)];
            Attack(attackUnit, moveLocation);
        }
        else
        {
            Attack(attackUnit, attackUnit.Location);
        }
    }

    public override bool ActionStart(BattleUnit attackUnit, List<BattleUnit> hits, Vector2 coord)
    {
        attackUnit.AnimatorSetBool("isMove", true);
        BattleManager.Instance.SetTlieClickCoolDown(1.1f);
        GameManager.Instance.PlayAfterCoroutine(() => {
            attackUnit.AnimatorSetBool("isMove", false);

            if (coord != attackUnit.Location)
            {
                SpawnData sd = new();
                sd.unitData = GameManager.Resource.Load<UnitDataSO>($"ScriptableObject/UnitDataSO/¸Á°¢ÀÇ_´ýºÒ");
                sd.location = attackUnit.Location;
                sd.team = attackUnit.Team;

                BattleManager.Field.ExitTile(attackUnit.Location);
                BattleManager.Field.EnterTile(attackUnit, coord);
                attackUnit.SetLocation(coord);

                BattleUnit summonUnit = BattleManager.Spawner.SpawnDataSpawn(sd);
                summonUnit.SetFlipX(attackUnit.GetFlipX());
            }

            List<BattleUnit> hitUnits = BattleManager.Field.GetUnitsInRange(attackUnit.Location, UDLR, attackUnit.Team == Team.Player ? Team.Enemy : Team.Player);
            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Data.ID == "¸Á°¢ÀÇ_´ýºÒ" && unit.Team == attackUnit.Team)
                {
                    unit.AnimatorSetBool("isAttack", true);
                    GameManager.Instance.PlayAfterCoroutine(() => {
                        unit.AnimatorSetBool("isAttack", false);
                    }, 1.5f);
                    hitUnits.AddRange(BattleManager.Field.GetUnitsInRange(unit.Location, UDLR, unit.Team == Team.Player ? Team.Enemy : Team.Player));
                }
            }

            BattleManager.Instance.AttackStart(attackUnit, hitUnits.Distinct().ToList());
        }, 1f);

        return true;
    }

    public override bool ActionTimingCheck(ActiveTiming activeTiming, BattleUnit caster, BattleUnit receiver)
    {
        if ((activeTiming & ActiveTiming.FALLED) == ActiveTiming.FALLED)
        {
            if (BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "¾Æ¶óº§¶ó" && findUnit != caster) != null)
            {
                while (true)
                {
                    BattleUnit remainUnit = BattleManager.Data.BattleUnitList.Find(findUnit => findUnit.Data.ID == "¸Á°¢ÀÇ_´ýºÒ" && findUnit.Team == caster.Team);
                    if (remainUnit == null)
                        break;

                    remainUnit.UnitDiedEvent(false);
                }
            }
        }
        else if ((activeTiming & ActiveTiming.ATTACK_TURN_START) == ActiveTiming.ATTACK_TURN_START)
        {
            List<Vector2> tileList = new();

            foreach (BattleUnit unit in BattleManager.Data.BattleUnitList)
            {
                if (unit.Team == caster.Team)
                    continue;

                foreach (Vector2 direction in UDLR)
                {
                    Vector2 location = unit.Location + direction;
                    if (BattleManager.Field.IsInRange(location) && !BattleManager.Field.TileDict[location].UnitExist && !tileList.Contains(location))
                    {
                        tileList.Add(location);
                    }
                }
            }

            foreach (Vector2 tile in tileList)
            {
                if (BattleManager.Field.IsInRange(tile))
                {
                    BattleManager.Field.TileDict[tile].IsColored = true;
                    BattleManager.Field.TileDict[tile].SetColor(BattleManager.Field.ColorList(FieldColorType.Attack));
                }
            }
        }

        return false;
    }

    public override List<Vector2> GetSplashRangeForField(BattleUnit unit, Tile targetTile, Vector2 caster)
    {
        List<Vector2> splashRangeList = new();
        Vector2 target = BattleManager.Field.GetCoordByTile(targetTile);
        foreach (Vector2 vec in unit.GetAttackRange())
        {
            if (BattleManager.Field.IsInRange(vec + target))
                splashRangeList.Add(vec + target);
        }

        return splashRangeList;
    }
}