using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaitingLine : UI_Scene
{
    [SerializeField] private Transform _grid;
    [SerializeField] private GameObject _buttonDown;
    [SerializeField] private GameObject _buttonUp;

    private List<UI_WaitingUnit> _waitingUnitList = new();
    private bool _turned = false;

    public void Start()
    {
        _buttonDown.SetActive(false);
        _buttonUp.SetActive(false);
    }

    private void ButtonActive()
    {
        if (_waitingUnitList.Count > 5)
        {
            if (_turned && _waitingUnitList.Count > 5)
            {
                _buttonUp.SetActive(true);
                _buttonDown.SetActive(false);
            }
            else if (!_turned && _waitingUnitList.Count > 5)
            {
                _buttonDown.SetActive(true);
                _buttonUp.SetActive(false);
            }
        }
        else
        {
            _buttonDown.SetActive(false);
            _buttonUp.SetActive(false);
        }
    }

    private void AddUnit(BattleUnit addUnit)
    {
        UI_WaitingUnit newUnit = GameManager.Resource.Instantiate("UI/Sub/WaitingUnit", _grid).GetComponent<UI_WaitingUnit>();
        newUnit.SetUnit(addUnit, _turned);
        _waitingUnitList.Add(newUnit);
    }

    public void SetWaitingLine(List<BattleUnit> orderList)
    {
        ClearWaitingLine();

        foreach (BattleUnit unit in orderList)
            AddUnit(unit);

        ButtonActive();
    }

    private void ClearWaitingLine()
    {
        foreach (UI_WaitingUnit unit in _waitingUnitList)
        {
            Destroy(unit.gameObject);
        }
        _waitingUnitList.Clear();
    }

    public void OnClickSeeNextUnits()
    {
        ButtonActive();

        _grid.eulerAngles += new Vector3(0f, 180f, 0f);
        if (_turned == false)
            _turned = true;
        else
            _turned = false;
    }
}
