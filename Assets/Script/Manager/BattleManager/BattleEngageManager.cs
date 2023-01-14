using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEngageManager : MonoBehaviour
{
    BattleCutSceneManager _CutSceneMNG;
    BattleDataManager _BattleDataMNG;

    List<BattleUnit> _BattleUnitList;

    // 턴 시작이 가능한 상태인가?
    bool CanTurnStart = true;
    int _unitListIndex = 0;


    private void Start()
    {
        _CutSceneMNG = GameManager.Instance.BattleMNG.CutSceneMNG;
        _BattleDataMNG = GameManager.Instance.BattleMNG.BattleDataMNG;

        _BattleUnitList = _BattleDataMNG.BattleUnitMNG.BattleUnitList;
    }


    // 턴 진행
    public void TurnStart()
    {
        // 턴 시작이 가능한 상태라면
        if (CanTurnStart)
        {
            CanTurnStart = false;
            _unitListIndex = 0;
            
            // 턴 시작 전에 다시한번 순서를 정렬한다.
            _BattleDataMNG.BattleUnitMNG.BattleOrderReplace();
            _BattleDataMNG.FieldMNG.FieldClear();

            UseUnitSkill();
        }
    }

    // BattleUnitList의 첫 번째 요소부터 순회
    // 다음 차례의 공격 호출은 CutSceneMNG의 ZoomOut에서 한다.
    // 나중에 죽이는 로직 구현 필요
    public void UseUnitSkill()
    {
        // index가 리스트의 범위를 넘지 않는다면 use를 실행
        if (_unitListIndex < _BattleUnitList.Count)
        {
            if (0 < _BattleUnitList[_unitListIndex].UnitAction.CurHP)
            {
                _BattleUnitList[_unitListIndex].use();
                _unitListIndex++;
            }
            else
            {
                _unitListIndex++;
                UseUnitSkill();
            }
        }
        else
        {
            TurnEnd();
        }

    }

    void TurnEnd()
    {
        DestroyDeadUnit();
        _BattleDataMNG.ManaMNG.AddMana(2);
        CanTurnStart = true;
    }

    void DestroyDeadUnit()
    {
        foreach (BattleUnit unit in _BattleUnitList)
        {
            if (unit.UnitAction.CurHP <= 0)
            {
                unit.UnitAction.UnitDestroy();
            }
        }
    }
}
