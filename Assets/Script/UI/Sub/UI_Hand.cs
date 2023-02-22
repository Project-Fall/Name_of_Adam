using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Hand : UI_Base
{
    private Unit _handUnit = null;
    private GameObject _highlight = null;


    private void Start()
    {
        _highlight = Util.FindChild(gameObject, "Highlight");
        _highlight.SetActive(false);
    }

    public void SetHandUnit(Unit unit)
    {
        _handUnit = unit;
        SetUnitInfo();
    }

    private void SetUnitInfo()
    {
        // UI가 완성된 후에 디테일한 요소 추가
    }

    public Unit GetHandUnit()
    {
        return _handUnit;
    }

    public bool IsHandNull()
    {
        if (_handUnit == null)
            return true;
        else
            return false;
    }

    private void OnMouseDown() 
    {
        Debug.Log("Hand Click");
        //_hands.OnHandClick(this);
    }

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}
