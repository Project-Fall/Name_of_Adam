using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_AI_Controller : MonoBehaviour
{
    protected BattleDataManager _Data;
    protected Field _field;

    protected BattleUnit caster;

    //공격 범위: 움직이지 않고 공격할 수 있는 범위; Attack Range
    //공격가능 타일: 해당 타일로 이동할 경우 공격할 수 있는 타일; Attackable Tile

    protected List<Vector2> AttackRangeUnitList = new List<Vector2>();
    //공격 범위 내 유닛

    protected List<Vector2> AttackableTileList = new List<Vector2>();
    //공격 가능 타일 

    protected List<Vector2> UnitAttackableTileList = new List<Vector2>();
    //공격 가능 + 사거리 내 타일

    protected List<Vector2> FourWay = new List<Vector2> { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
    //상하좌우 foreach용

    protected Dictionary<Vector2, int> TileHPDict = new Dictionary<Vector2, int>();

    void Awake()
    {
        _Data = GameManager.Battle.Data;
        _field = GameManager.Battle.Field;
    }

    public void SetCaster(BattleUnit unit)
    {
        caster = unit;
    }

    private void SetAttackRangeList()
    {
        //캐스터의 공격 범위 내에 있는 유닛을 리스트에 담는다.
        foreach (Vector2 attackRange in caster.GetAttackRange())
        {
            Vector2 range = caster.Location + attackRange;

            if (!_field.IsInRange(range))
            {
                continue;
            }

            if (_field.TileDict[range].UnitExist && _field.TileDict[range].Unit.Team == Team.Player)
            {
                AttackRangeUnitList.Add(range);

                if (TileHPDict.ContainsKey(range))
                {
                    if (TileHPDict[range] >= _field.TileDict[range].Unit.Stat.HP)
                    {
                        TileHPDict.Remove(range);
                        TileHPDict.Add(range, _field.TileDict[range].Unit.Stat.HP);
                    }
                }
                else
                {
                    TileHPDict.Add(range, _field.TileDict[range].Unit.Stat.HP);
                }
            }
        }
    }

    protected void SetAttackableTile()
    {
        //모든 공격가능 타일을 리스트에 저장한다.
        foreach (BattleUnit unit in _Data.BattleUnitList)
        {
            if (unit.Team == Team.Player)
            {
                foreach (Vector2 range in caster.GetAttackRange())
                {
                    //공격가능 타일은 공격하는 유닛의 공격 범위의 점 대칭이다. 따라서 -.
                    Vector2 attackableRange = unit.Location - range;
                    AttackableTileList.Add(attackableRange);

                    if (TileHPDict.ContainsKey(attackableRange))
                    {
                        if (TileHPDict[attackableRange] >= unit.Stat.HP)
                        {
                            TileHPDict.Remove(attackableRange);
                            TileHPDict.Add(attackableRange, unit.Stat.HP);
                        }
                    }
                    else
                    {
                        TileHPDict.Add(attackableRange, unit.Stat.HP);
                    }
                }
            }
        }
    }

    private void AttackableTileSearch()
    {
        //캐스터의 공격 범위 내에 있는 유닛을 리스트에 담는다.
        List<Vector2> swapList = new List<Vector2>();

        foreach (Vector2 moveRange in caster.GetMoveRange())
        {
            Vector2 range = caster.Location + moveRange;

            if (!_field.IsInRange(range))
            {
                continue;
            }

            if (AttackableTileList.Contains(range))
            {
                if (_field.TileDict[range].UnitExist)
                {
                    swapList.Add(range);
                }
                else
                {
                    UnitAttackableTileList.Add(range);
                }
            }

            if (UnitAttackableTileList.Count == 0)
            {
                foreach (Vector2 vec in swapList)
                    UnitAttackableTileList.Add(vec);
            }
        }
    }

    private Vector2 MinHPSearch(List<Vector2> vecList)
    {
        //리스트에서 가장 체력이 낮은 적을 찾는다.
        List<Vector2> minHPList = new List<Vector2>();

        int minHP = TileHPDict[vecList[0]];

        foreach (Vector2 unit in vecList)
        {
            int currentHP = TileHPDict[unit];

            if (minHP > currentHP)
            {
                minHP = currentHP;
                minHPList.Clear();
                minHPList.Add(unit);
            }
            else if (minHP == currentHP)
            {
                minHPList.Add(unit);
            }
        }

        return minHPList[Random.Range(0, minHPList.Count)];
    }

    protected void MoveUnit(Vector2 moveVector)
    {


        _field.MoveUnit(caster.Location, moveVector);
    }

    private void Attack(BattleUnit unit)
    {
        caster.SkillUse(unit);
    }

    protected Vector2 NearestEnemySearch()
    {
        Vector2 MyPosition = caster.Location;

        float minDistance = 100f;

        List<Vector2> nearestEnemy = new List<Vector2>();

        foreach (Vector2 vec in AttackableTileList)
        {
            float abs = Mathf.Abs(vec.x - MyPosition.x) + Mathf.Abs(vec.y - MyPosition.y);
            if (minDistance > abs)
            {
                minDistance = abs;
                nearestEnemy.Clear();
                nearestEnemy.Add(vec);
            }
            else if (minDistance == abs)
            {
                nearestEnemy.Add(vec);
            }
        }

        return nearestEnemy[Random.Range(0, nearestEnemy.Count)];
    }

    public Vector2 MoveDirection(Vector2 destination)
    {
        //가야하는 위치 destination을 받아 상하좌우 중 어디로 갈지를 정해 moveVec으로 리턴한다
        Vector2 MyPosition = caster.Location;
        float currntMin = 100f;

        List<Vector2> moveVectorList = new List<Vector2>();

        foreach (Vector2 direction in FourWay)
        {
            Vector2 Vec = new Vector2(MyPosition.x + direction.x, MyPosition.y + direction.y);
            float sqr = (Vec - destination).sqrMagnitude;

            if (currntMin > sqr)
            {
                currntMin = sqr;
                moveVectorList.Clear();
                if (!_field.TileDict[Vec].UnitExist)
                {
                    moveVectorList.Add(Vec);
                }
            }
            else if (currntMin == sqr)
            {
                if (!_field.TileDict[Vec].UnitExist)
                {
                    moveVectorList.Add(Vec);
                }
            }
        }

        if (moveVectorList.Count == 0)
        {
            return MyPosition;
        }
        else
        {
            return moveVectorList[Random.Range(0, moveVectorList.Count)];
        }
    }

    protected void ListClear()
    {
        AttackRangeUnitList.Clear();
        AttackableTileList.Clear();
        UnitAttackableTileList.Clear();
        TileHPDict.Clear();
    }

    public virtual Vector2 AIMove()
    {
        ListClear();
        SetAttackRangeList();

        if (AttackRangeUnitList.Count > 0)
        {
            return caster.Location;
            //Attack(_field.TileDict[MinHPSearch(AttackRangeUnitList)].Unit);
        }
        else
        {
            SetAttackableTile();
            AttackableTileSearch();

            if (UnitAttackableTileList.Count > 0)
            {
                return MinHPSearch(UnitAttackableTileList);
                //MoveUnit(MinHPSearch(UnitAttackableTileList));

                //SetAttackRangeList();
                //Attack(_field.TileDict[MinHPSearch(AttackRangeUnitList)].Unit);
            }
            else
            {
                return MoveDirection(NearestEnemySearch());
                //MoveUnit(MoveDirection(NearestEnemySearch()));
            }
        }
    }

    public virtual Vector2 AISkillUse()
    {
        ListClear();
        SetAttackRangeList();

        if (AttackRangeUnitList.Count > 0)
        {
            return MinHPSearch(AttackRangeUnitList);
        }
        else
        {
            return caster.Location;
        }
    }
}

public class Common_Unit_AI_Controller : Unit_AI_Controller
{
    public override Vector2 AIMove()
    {
        return base.AIMove();
    }

    public override Vector2 AISkillUse()
    {
        return base.AISkillUse();
    }
}