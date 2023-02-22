using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaitingLine : UI_Scene
{
    [SerializeField] GameObject P_WaitingUnit;
    private List<UI_WaitingUnit> _waitingUnitList = new List<UI_WaitingUnit>();
    private Transform _grid;
    private const int _maxSize = 6;

    #region BattleUnitList  
    List<BattleUnit> _BattleUnitOrderList;
    #endregion

    private BattleManager _BattleMNG;

    public void Start()
    {
        _grid = Util.FindChild(gameObject, "Grid").transform;
        _BattleMNG = GameManager.Battle;
    }

    IEnumerator Test()
    {
        for(int i=0; i<6; i++)
        {
            UI_WaitingUnit newUnit = GameObject.Instantiate(P_WaitingUnit, _grid).GetComponent<UI_WaitingUnit>();
            _waitingUnitList.Add(newUnit);
            yield return new WaitForSeconds(1f);
        }
    }

    private void ClearLine()
    {
        for(int i= _waitingUnitList.Count-1; i>=0; i--)
        {
            UI_WaitingUnit unit = _waitingUnitList[i];
            _waitingUnitList.Remove(unit);
            Destroy(unit.gameObject);
        }
    }

    public void SetBattleOrderList()
    {
        _BattleUnitOrderList = _BattleMNG.GetUnitbyOrder();
    }

    public void SetWaitingLine()
    {
        ClearLine();

        for (int i = 0; i < _BattleUnitOrderList.Count; i++)
        {
            UI_WaitingUnit newUnit = GameObject.Instantiate(P_WaitingUnit, _grid).GetComponent<UI_WaitingUnit>();
            _waitingUnitList.Add(newUnit);
            _waitingUnitList[i].SetUnit(_BattleUnitOrderList[i].Data.Image);
        }
    }
}
