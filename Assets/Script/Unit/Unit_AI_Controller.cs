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

    protected List<Vector2> Attack_Range_Unit_List = new List<Vector2>();
    //공격 범위 내에 있는 일반 유닛의 타일 리스트
    protected List<Vector2> Attack_Range_Priority_Unit_List = new List<Vector2>();
    //공격 범위 내에 있는 우선 순위 유닛의 타일 리스트

    protected List<Vector3> Attackable_Tile_List = new List<Vector3>();
    //공격가능 타일의 리스트. 맵 위의 모든 리스트가 저장됨

    protected List<Vector3> Unit_Attackable_Tile_List = new List<Vector3>();
    //일반 유닛을 공격할 수 있는 이동 범위 내에 있는 공격가능 타일
    protected List<Vector3> Priority_Unit_Attackable_Tile_List = new List<Vector3>();
    //우선순위 유닛을 공격할 수 있는 이동 범위 내에 있는 공격가능 타일

    protected List<Vector2> FourWay = new List<Vector2>{new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1)};
    //상하좌우 foreach용

    void Awake()
    {
        _Data = GameManager.Battle.Data;
        _field = GameManager.Battle.Field;
    }

    public void SetCaster(BattleUnit unit)
    {
        caster = unit;
    }

    protected void AttackRangeSearch()
    {
        //현재 위치에서 공격범위 내의 유닛을 찾는다.
        foreach (Vector2 range in caster.Data.GetAttackRange())
        {
            Vector2 AttackRange = caster.Location + range;

            if (_field.IsInRange(AttackRange))
            {
                if (_field.TileDict[AttackRange].UnitExist && _field.TileDict[AttackRange].Unit.Team == Team.Player)
                {
                    Attack_Range_Unit_List.Add(AttackRange);
                }
            }
        }

        foreach (Vector2 unitTile in Attack_Range_Unit_List)
        {
            if (_field.TileDict[unitTile].Unit.Data.BehaviorType == BehaviorType.원거리)
            {
                Attack_Range_Priority_Unit_List.Add(unitTile);
            }
        }
        //Attack_Range_Unit_List에 유닛이 있는 타일을 모두 저장
        //Attack_Range_Priority_Unit_List엔 원거리 유닛이 있는 타일만 저장
    }

    protected void SetAttackableTile()
    {
        //모든 공격가능 타일을 Attackable_Tile_List에 저장한다. X, Y는 좌표, Z는 원거리/근거리 유무
        foreach (BattleUnit unit in _Data.BattleUnitList)
        {
            if (unit.Team == Team.Player)
            {
                foreach (Vector2 range in caster.Data.GetAttackRange())
                {
                    //공격가능 타일은 공격하는 유닛의 공격 범위의 점 대칭이다. 따라서 -.
                    
                    Vector3 vector = unit.Location - range;

                    if (unit.Data.BehaviorType == BehaviorType.원거리)
                        vector.z = 0f;//원거리면 0
                    else
                        vector.z = 0.1f;//근거리면 0.1
                    
                    Attackable_Tile_List.Add(vector);
                }
            }
        }
    }

    protected void AttackableTileSearch()
    {
        //유닛을 때릴 수 있는 타일이 이동 범위 내에 있는 지 확인한다.
        //단 위, 아래, 왼, 오른쪽만 이동 가능하다고 가정
        List<Vector3> swapList = new List<Vector3>();

        foreach(Vector2 vec in caster.Data.GetMoveRange())
        {
            for (float i = 0; i <= 0.1f; i += 0.1f)
            {
                Vector3 vec1 = new Vector3(caster.Location.x + vec.x, caster.Location.y + vec.y, i);
                if (Attackable_Tile_List.Contains(vec1))
                {
                    if (_field.TileDict[vec1].UnitExist)
                        swapList.Add(vec1);
                    else
                        Unit_Attackable_Tile_List.Add(vec1);
                }
            }
        }

        //스왑이 필요한지 확인
        if (Unit_Attackable_Tile_List.Count == 0)
        {
            foreach (Vector3 vec in swapList)
                Unit_Attackable_Tile_List.Add(vec);
        }

        foreach (Vector3 v in Unit_Attackable_Tile_List)
        {
            //원거리인지 확인
            if (v.z == 0)
            {
                Priority_Unit_Attackable_Tile_List.Add(new Vector3(v.x, v.y, 0));
            }
        }
    }

    protected Vector2 NearestEnemySearch()
    {
        Vector2 MyPosition = caster.Location;

        float dis = 100f;

        List<Vector3> list_minVec = new List<Vector3>();

        foreach (Vector3 v in Attackable_Tile_List)
        {
            float abs = Mathf.Abs(v.x - MyPosition.x) + Mathf.Abs(v.y - MyPosition.y);
            if (dis > abs)
            {
                dis = abs;
                list_minVec.Clear();
                list_minVec.Add(v);
            }
            else if (dis == abs)
            {
                list_minVec.Add(v);
            }
        }
        Vector3 minVec = list_minVec[Random.Range(0, list_minVec.Count)];

        return minVec;
    }

    public Vector2 MoveDirection(Vector2 minVec)
    {
        //가야하는 위치 minVec을 받아 상하좌우 중 어디로 갈지를 정해 moveVec으로 리턴한다
        Vector2 MyPosition = caster.Location;
        float currntMin = 100f;

        List<Vector2> moveVectorList = new List<Vector2>();

        foreach (Vector2 direction in FourWay)
        {
            Vector2 Vec = new Vector2(MyPosition.x + direction.x, MyPosition.y + direction.y);
            float sqr = (Vec - minVec).sqrMagnitude;

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

    protected Vector2 UnitCoord(List<Vector2> UnitList)
    {
        return UnitList[Random.Range(0, UnitList.Count)];
    }

    protected Vector2 UnitCoord(List<Vector3> UnitList)
    {
        return UnitList[Random.Range(0, UnitList.Count)];
    }

    protected void MoveUnit(Vector2 moveVector)
    {
        _field.MoveUnit(caster.Location, moveVector);
    }

    protected void Attack()
    {
        if (Attack_Range_Priority_Unit_List.Count > 0)
        {
            //원거리 유닛이 있을 경우
            //caster.AttackTileClick(UnitCoord(Attack_Range_Priority_Unit_List));  임시임시임시임시임시
            caster.SkillUse(_field.TileDict[UnitCoord(Attack_Range_Priority_Unit_List)].Unit);
        }
        else
        {
            //근거리 유닛만 있을 경우
            //caster.AttackTileClick(UnitCoord(Attack_Range_Unit_List));  임시임시임시임시임시임시임시
            caster.SkillUse(_field.TileDict[UnitCoord(Attack_Range_Unit_List)].Unit);
        }
    }

    protected void ListClear()
    {
        Attack_Range_Unit_List.Clear();
        Attack_Range_Priority_Unit_List.Clear();

        Attackable_Tile_List.Clear();
        
        Unit_Attackable_Tile_List.Clear();
        Priority_Unit_Attackable_Tile_List.Clear();
    }

    public virtual void AIAction()
    {
        //전달받은 범위에서 유닛을 찾는다.
        AttackRangeSearch();

        //찾은 유닛이 있는지 확인하고, 있다면 원거리인지, 근거리인지 확인한다.
        if (Attack_Range_Unit_List.Count > 0)
        {
            Debug.Log("공격 범위 내 유닛");
            Attack();
        }
        else
        {
            //공격 범위 내에서 찾은 유닛이 없으면 이동하고 공격한다
            SetAttackableTile();

            AttackableTileSearch();

            if (Unit_Attackable_Tile_List.Count > 0)
            {
                if (Priority_Unit_Attackable_Tile_List.Count > 0)
                {
                    //원거리가 있음
                    Debug.Log("공격가능 타일에 원거리");
                    MoveUnit(UnitCoord(Priority_Unit_Attackable_Tile_List));
                    AttackRangeSearch();
                    Attack();
                }
                else
                {
                    //근거리만 있음
                    Debug.Log("공격가능 타일에 근거리");
                    MoveUnit(UnitCoord(Unit_Attackable_Tile_List));
                    AttackRangeSearch();
                    Attack();
                }
            }
            else
            {
                Debug.Log("공격 불가 이동");
                MoveUnit(MoveDirection(NearestEnemySearch()));
            }
        }
        ListClear();
    }
}

public class Common_Unit_AI_Controller : Unit_AI_Controller
{
    public override void AIAction()
    {
        base.AIAction();
    }
}