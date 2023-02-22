using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurnCount : UI_Scene
{
    enum Objects
    {
        TurnCountText
    }

    private BattleDataManager _data;

    void Start()
    {
        Bind<GameObject>(typeof(Objects));
        _data = GameManager.Battle.Data;
        GetObject((int)Objects.TurnCountText).GetComponent<Text>().text = _data.TurnCount.ToString();
    }

    public void ShowTurn()
    {
        GetObject((int)Objects.TurnCountText).GetComponent<Text>().text = _data.TurnCount.ToString();
    }
}
