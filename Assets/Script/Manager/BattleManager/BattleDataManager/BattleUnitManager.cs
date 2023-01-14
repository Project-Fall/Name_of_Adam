using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager
{
    // 전투를 진행중인 캐릭터가 들어있는 리스트
    #region BattleUnitList  
    List<BattleUnit> _BattleUnitList = new List<BattleUnit>();
    public List<BattleUnit> BattleUnitList => _BattleUnitList;
    #endregion  

    // 리스트에 캐릭터를 추가 / 제거
    #region UnitEnter / Exit
    public void BattleUnitEnter(BattleUnit unit)
    {
        BattleUnitList.Add(unit);
    }
    public void BattleUnitExit(BattleUnit unit)
    {
        BattleUnitList.Remove(unit);
    }
    #endregion

    // 현재 리스트를 초기화
    public void UnitListClear()
    {
        BattleUnitList.Clear();
    }
    
    // BattleUnitList를 정렬
    // 1. 스피드 높은 순으로, 2. 같을 경우 왼쪽 위부터 오른쪽으로 차례대로
    public void BattleOrderReplace()
    {
        _BattleUnitList = BattleUnitList.OrderByDescending(unit => unit.GetSpeed())
                                        .ThenByDescending(unit => unit.UnitMove.LocY)
                                        .ThenBy(unit => unit.UnitMove.LocX)
                                        .ToList();

        //foreach(BattleUnit t in BattleUnitList)
        //{
        //    Debug.Log("Speed : " + t.GetSpeed() + ", Y : " + t.LocY + ", X : " + t.LocX);
        //}
    }

    //필드에 유닛을 생성
    public void CreatBattleUnit(GameObject BattleUnitPrefab, int x, int y)
    {                   
        BattleUnit BattleUnit = BattleUnitPrefab.GetComponent<BattleUnit>();

        BattleUnit.BattleUnitSO = GameManager.Instance.InputMNG.ClickedUnit.GetUnitSO();
        BattleUnit.UnitMove.setLocate(x, y);

        BattleUnitEnter(BattleUnit);
        GameManager.Instance.BattleMNG.BattleDataMNG.FieldMNG.EnterTile(BattleUnit, x, y);
                        
        
    }

}
