using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct SpawnData
{
    public GameObject prefab;
    public Vector2 location;
    public Team team;
    public Passive[] stigmas;
}

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] List<SpawnData> SpawnMonsters;

    public void Spawn()
    {
        Transform parent = SetParent();

        foreach(SpawnData data in SpawnMonsters)
        {
            if (GameManager.Battle.Field.TileDict[data.location].UnitExist)
            {
                Debug.Log("해당 타일에 유닛이 존재합니다.");
                continue;
            }

            GameObject go = GameObject.Instantiate(data.prefab, parent);
            GameManager.Battle.UnitSetting(go.GetComponent<BattleUnit>(), data.location);
        }
    }

    private Transform SetParent()
    {
        GameObject go = GameObject.Find("Units");
        if (go == null)
            go = new GameObject("Units");
        return go.transform;
    }
}
