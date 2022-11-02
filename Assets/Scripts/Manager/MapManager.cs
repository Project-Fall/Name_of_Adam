using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Manager<MapManager>
{
    public GameObject Map; // Find를 하지 않기 위한 임시방편
    private Tile[,] _map = new Tile[8, 3];

    // 나중에는 맵을 생성하는 용도로 쓰일 것 같음
    // 현재는 이미 만들어둔 맵 관리...?

    public void Init()
    {
        if(Map == null)
        {
            Map = GameObject.Find("Map");
        }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 8; j++)
                _map[j, i] = Map.transform.GetChild(i * 8 + j).GetComponent<Tile>();
    }

    public void Hover(int PosX, int PosY)
    {

    }
}
