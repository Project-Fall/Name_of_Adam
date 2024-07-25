using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI_WaitingLine : UI_Scene
{
    [SerializeField] private Transform _waitingUnitGrid;
    [SerializeField] private GameObject _upButton;
    [SerializeField] private GameObject _downButton;

    [SerializeField] private GameObject _waitingUnitPrefab;

    private List<UI_WaitingUnit> _waitingUnitList = new();
    private List<BattleUnit> _waitingBattleUnitList = new();
    private int _waitingLinePage = 0;

    public void Start()
    {
        _downButton.SetActive(false);
        _upButton.SetActive(false);
    }

    private void ButtonActive()
    {
        if (_waitingUnitList.Count > 5)
        {
            _upButton.SetActive(_waitingLinePage != 0);
            _downButton.SetActive(_waitingUnitList.Count / 5 != _waitingLinePage);
        }
        else
        {
            _downButton.SetActive(false);
            _upButton.SetActive(false);
            _waitingLinePage = 0;
        }

        CheckWaitingLineActive();
    }

    private void AddUnit(BattleUnit addUnit)
    {
        UI_WaitingUnit newUnit = GameObject.Instantiate(_waitingUnitPrefab, _waitingUnitGrid).GetComponent<UI_WaitingUnit>();
        newUnit.SetUnit(addUnit);
        _waitingUnitList.Add(newUnit);
    }

    public void SetWaitingLine(List<BattleUnit> orderList)
    {
        if (_waitingBattleUnitList.SequenceEqual(orderList) && _waitingBattleUnitList.Select(unit => unit.Team).SequenceEqual(orderList.Select(unit => unit.Team)))
        {
            return;
        }

        ClearWaitingLine();

        foreach (BattleUnit unit in orderList)
            AddUnit(unit);

        ButtonActive();

        _waitingBattleUnitList = orderList;
    }

    private void ClearWaitingLine()
    {
        foreach (UI_WaitingUnit unit in _waitingUnitList)
        {
            Destroy(unit.gameObject);
        }
        _waitingUnitList.Clear();
    }

    private void CheckWaitingLineActive()
    {
        for (int i = 0; i < _waitingUnitList.Count; i++)
        {
            _waitingUnitList[i].gameObject.SetActive(i >= _waitingLinePage * 5);
        }
    }

    public void OnClickUpButton()
    {
        _waitingLinePage--;
        ButtonActive();
    }

    public void OnClickDownButton()
    {
        _waitingLinePage++;
        ButtonActive();
    }
}
