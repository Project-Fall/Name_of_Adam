using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Hands : UI_Scene
{
    [SerializeField] GameObject HandPrefabs;
    private List<UI_Hand> _handList = new List<UI_Hand>();
    private Transform _grid;

    private const int _maxSize = 3;

    void Start()
    {
        _grid = Util.FindChild(gameObject, "Grid").transform;
        _handList = new List<UI_Hand>();

        //_Data = GameManager.Battle.Data;

        //for (int i = 0; i < HandSize; i++)
        //{
        //    GameObject obj = Instantiate(HandPrefabs, transform);
        //    obj.transform.position = new Vector3(6 + 1.5f, -5.3f);

        //    _handList.Add(obj.GetComponent<UI_Hand>());
        //}
    }

    IEnumerator Test()
    {
        for (int i = 0; i < 3; i++)
        {
            UI_Hand newUnit = GameObject.Instantiate(HandPrefabs, _grid).GetComponent<UI_Hand>();
            _handList.Add(newUnit);
            yield return new WaitForSeconds(1f);
        }
    }

    public void AddUnit(Unit unit)
    {
        UI_Hand newCard = GameObject.Instantiate(HandPrefabs, _grid).GetComponent<UI_Hand>();
        newCard.SetHandUnit(unit);
        _handList.Add(newCard);
    }

    public void RemoveUnit(Unit unit)
    {
        UI_Hand card = FindCardByUnit(unit);

        if (card != null)
            DestroyCard(card);
    }

    public void ClearHands()
    {
        foreach (UI_Hand card in _handList)
            DestroyCard(card);
    }

    private void DestroyCard(UI_Hand card)
    {
        _handList.Remove(card);
        Destroy(card.gameObject);
    }

    private UI_Hand FindCardByUnit(Unit unit)
    {
        foreach(UI_Hand card in _handList)
        {
            if (card.GetHandUnit() == unit)
                return card;
        }

        return null;
    }

    public Unit RemoveHand(int handIndex)
    {
        ////handIndex는 1부터 시작하기에 -1 해야함
        //Unit returnUnit;

        //returnUnit = _handList[handIndex].RemoveHandUnit();

        //for (int i = handIndex + 1; i < HandSize; i++)
        //{
        //    _handList[i - 1].SetHandUnit(_handList[i].RemoveHandUnit());
        //}

        ////빈 공간이 있으면 1개 추가
        //AddUnitToHand();

        //return returnUnit;
        return null;
    }

    #region Hand Click
    private int _ClickedHand = 0;
    public int ClickedHand => _ClickedHand;

    private Unit _ClickedUnit = null;
    public Unit ClickedUnit => _ClickedUnit;

    public void OnHandClick(UI_Hand hand)
    {
        _ClickedHand = _handList.IndexOf(hand);
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
