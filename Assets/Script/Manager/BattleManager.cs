using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    // 마나 UI
    [SerializeField] ManaGuage PlayerMana;
    // 전투가 진행되는 필드
    #region BattleField
    [SerializeField] Field _BattleField;
    public Field BattleField => _BattleField;
    #endregion

    // 전투를 진행중인 캐릭터가 들어있는 리스트
    List<Character> BattleCharList = new List<Character>();

    bool CanTurnStart = true;

    // 리스트에 캐릭터를 추가 / 제거
    #region CharEnter / Exit
    public void CharEnter(Character ch)
    {
        BattleCharList.Add(ch);
    }
    public void CharExit(Character ch)
    {
        BattleCharList.Remove(ch);
    }
    #endregion

    #region OrderSort

    void CharTurnReplace()
    {
        SpeedSort();
    }

    // 일단 선택 정렬으로 정렬, 나중에 바꾸기
    void SpeedSort()
    {
        for(int i = 0; i < BattleCharList.Count; i++)
        {
            Character max = null;
            for(int j = i; j < BattleCharList.Count; j++)
            {
                if (i == j)
                {
                    max = BattleCharList[j];
                }
                else if (BattleCharList[j].GetSpeed() > max.GetSpeed())
                {
                    CharSwap(i, j);
                }
                else if(BattleCharList[j].GetSpeed() == max.GetSpeed())
                {
                    if(BattleCharList[j].LocX < max.LocX)
                    {
                        CharSwap(i, j);
                    }
                    else if(BattleCharList[j].LocX == max.LocX)
                    {
                        if(BattleCharList[j].LocY < max.LocY)
                        {
                            CharSwap(i, j);
                        }
                    }
                }
            }
        }
    }

    void CharSwap(int a, int b)
    {
        Character dump = BattleCharList[a];
        BattleCharList[a] = BattleCharList[b];
        BattleCharList[b] = dump;
    }

    #endregion

    // 턴 진행
    public void TurnStart()
    {
        if (CanTurnStart)
        {
            CanTurnStart = false;
            CharTurnReplace();

            StartCoroutine(CharUse());
        }
    }
    //턴에 딜레이 주기(어떻게 줘야할까?)
    IEnumerator CharUse()
    {
        for (int i = 0; i < BattleCharList.Count; i++)
        {
            BattleCharList[i].use();

            yield return new WaitForSeconds(1f);
        }

        TurnEnd();
    }

    void TurnEnd()
    {
        PlayerMana.AddMana(2);
        CanTurnStart = true;
    }
}