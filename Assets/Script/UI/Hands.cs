using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    [SerializeField] List<Hand> HandList;

    void OnMouseDown()
    {
        //전투 시작 후 초기 멀리건 4장
        for (int i = 0; i < 4; i++) {
            AddUnitToHand();
        }
    }

    /*
    void Awake()
    {
        //전투 시작 후 초기 멀리건 4장
        for (int i = 0; i < 4; i++) {
            AddUnitToHand();
        }
    }
    */

    public void AddUnitToHand()
    {
        //1,2,3,4 순으로 Hand의 Unit이 null이면 1개 추가
        foreach (Hand h in HandList)
        {
            if (h.isHandNull())
            {
                h.SetHandDeckUnit(GameManager.Instance.BattleMNG.BattleDataMNG.GetRandomDeckUnit());
                break;
            }
        }
    }
    public DeckUnit RemoveHand(int handIndex)
    {
        //handIndex는 1부터 시작하기에 -1 해야함
        DeckUnit returnUnit;

        returnUnit = HandList[handIndex-1].RemoveHandDeckUnit();

        for (int i = (handIndex-1)+1; i < 4; i++)
        {
            HandList[i-1].SetHandDeckUnit(HandList[i].RemoveHandDeckUnit());
        }

        //빈 공간이 있으면 1개 추가
        AddUnitToHand();

        return returnUnit;
    }

    public void ReturnHand()
    {
        foreach (Hand h in HandList)
        {
            if (!h.isHandNull())
            {
                GameManager.Instance.BattleMNG.BattleDataMNG.AddDeckUnit(h.RemoveHandDeckUnit());
            }
        }
    }
}
