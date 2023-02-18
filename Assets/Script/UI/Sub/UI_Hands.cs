using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hands : UI_Scene
{
    [SerializeField] GameObject HandPrefabs;
    private List<UI_Hand> _HandList;
    
    BattleDataManager _Data;

    const int HandSize = 4;

    void Start()
    {
        _Data = GameManager.Battle.Data;

        _HandList = new List<UI_Hand>();

        for (int i = 0; i < HandSize; i++)
        {
            GameObject obj = Instantiate(HandPrefabs, transform);
            obj.transform.position = new Vector3(6 + 1.5f, -5.3f);

            _HandList.Add(obj.GetComponent<UI_Hand>());
        }
    }

    public void TEST()
    {
        Debug.Log("?");
        //전투 시작 후 초기 멀리건 4장
        for (int i = 0; i < HandSize; i++) {
            AddUnitToHand();
        }
    }

    public void AddUnitToHand()
    {
        //1,2,3,4 순으로 Hand의 Unit이 null이면 1개 추가
        foreach (UI_Hand h in _HandList)
        {
            if (h.IsHandNull())
            {
                h.SetHandUnit(_Data.GetRandomUnit());
                break;
            }
        }
    }
    public Unit RemoveHand(int handIndex)
    {
        //handIndex는 1부터 시작하기에 -1 해야함
        Unit returnUnit;

        returnUnit = _HandList[handIndex].RemoveHandUnit();

        for (int i = handIndex+1; i < HandSize; i++)
        {
            _HandList[i-1].SetHandUnit(_HandList[i].RemoveHandUnit());
        }

        //빈 공간이 있으면 1개 추가
        AddUnitToHand();

        return returnUnit;
    }

    public void ReturnHand()
    {
        foreach (UI_Hand h in _HandList)
        {
            if (!h.IsHandNull())
            {
                _Data.AddUnit(h.RemoveHandUnit());
            }
        }
        _HandList = null;
    }

    #region Hand Click
    private int _ClickedHand = 0;
    public int ClickedHand => _ClickedHand;

    private Unit _ClickedUnit = null;
    public Unit ClickedUnit => _ClickedUnit;

    public void OnHandClick(UI_Hand hand)
    {
        _ClickedHand = _HandList.IndexOf(hand);
        _ClickedUnit = hand.GetHandUnit();

        // Memo : 소환 이전에 클릭이 되지 않아야 함
        //if (!_Data.Mana.CanUseMana(_ClickedUnit.Data.ManaCost)){
        //    Debug.Log("not enough mana");
        //    ClearHand();
        //}
    }

    public void ClearHand()
    {
        _ClickedHand = 0;
        _ClickedUnit = null;
    }
    #endregion
}
