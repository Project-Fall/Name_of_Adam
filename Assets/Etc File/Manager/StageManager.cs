using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncountStage
{
    Monster = 1 << 0,
    Elite   = 1 << 1,
    Event   = 1 << 2,

    Boss
}

public class StageManager
{
    public EncountStage[] StageArray;

    // 각 종류의 스테이지가 얼마나 남았는지 관리
    Dictionary<EncountStage, int> _encountDic = new Dictionary<EncountStage, int>();

    public StageManager()
    {
        SetEncountDic();
    }

    void SetEncountDic()
    {
        _encountDic.Add(EncountStage.Monster, 4);
        _encountDic.Add(EncountStage.Elite, 2);
        _encountDic.Add(EncountStage.Event, 4);
    }

    public EncountStage[] GetStageArray()
    {
        StageArray = new EncountStage[3];
        return StageArray;
    }

    // 등장 가능한 스테이지를 남은 갯수만큼 리스트에 넣고 반환
    public List<EncountStage> GetStage()
    {
        List<EncountStage> EncountList = new List<EncountStage>();

        EncountStage flag = CheckEncount();

        // 남은 스테이지가 없을 경우 보스 스테이지로 간다
        if(flag == 0) // Boss
            return EncountList;

        foreach (EncountStage es in Enum.GetValues(typeof(EncountStage)))
        {
            if (es == EncountStage.Boss)
                continue;

            // Event는 남은 갯수가 없어도 2개는 있어야 한다
            if(es == EncountStage.Event)
            {
                if(_encountDic[es] <= 2)
                {
                    int eve = 2;

                    // Event밖에 안남았을 경우 모든 선택지를 채운다
                    if (flag == es)
                        eve = 3;

                    for (int i = 0; i < eve; i++)
                        EncountList.Add(es);
                }
                else
                {
                    for (int i = 0; i < _encountDic[es]; i++)
                        EncountList.Add(es);
                }
            }
            // 각 스테이지의 남은 갯수만큼 리스트에 더해준다
            else if((flag & es) == es)
            {
                for (int i = 0; i < _encountDic[es]; i++)
                    EncountList.Add(es);
            }
        }
        

        return EncountList;
    }

    // 등장할 수 있는 스테이지를 반환
    EncountStage CheckEncount()
    {
        EncountStage flag = new EncountStage();

        foreach (EncountStage es in Enum.GetValues(typeof(EncountStage)))
        {
            if (es == EncountStage.Boss)
                continue;
            if (0 < _encountDic[es])
            {
                flag |= es;
            }
        }

        return flag;
    }

    // 선택된 박스에 들어있는 스테이지로 진입
    public void StageSelect(int index)
    {
        // 박스에 들어있는 스테이지를 encount에 할당
        EncountStage encount = StageArray[index];

        foreach (EncountStage es in Enum.GetValues(typeof(EncountStage)))
        {
            if(es == encount)
            {
                if(es != EncountStage.Boss)
                    _encountDic[es]--;
                CreateStage(es);
            }
        }
        
        // CreateStage가 완성되면 지우기
        GameManager.SceneChanger.SceneChange("SampleScene");
    }

    void CreateStage(EncountStage stage)
    {
        // 새로 만들어지는 방의 정보

        // 전 스테이지에서 사용된 데이터를 모두 초기화
        // Memo : 이건 나중에 Stage가 꺼지기 전에 하면 좋을듯
        GameManager.Battle.Data.UnitListClear();

        // 방 생성은 여기서? 아니면 배틀매니저에서?

        Debug.Log("Monster : " + _encountDic[EncountStage.Monster]
            + ", Elite : " + _encountDic[EncountStage.Elite]
            + ", Event : " + _encountDic[EncountStage.Event]);
    }

}