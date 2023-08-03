using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StigmaController
{
    public enum StigmaManageType
    {
        Entire,
        Unique,
        Common,
        Locked
    }

    private Dictionary<StigmaManageType, List<Stigma>> _stigmaDict = new();
    public Dictionary<StigmaManageType, List<Stigma>> StigmaDict => _stigmaDict;
    private List<Stigma> _weightStigmaList = new();

    public StigmaController()
    {
        LoadStigmaList();
    }

    private void LoadStigmaList()
    {
        List<Stigma> common = new();
        List<Stigma> unique = new();
        List<Stigma> locked = new();
        List<Stigma> entire = new();

        string path = "Assets/Resources/Prefabs/Stigma";
        string[] files = Directory.GetFiles(path, "*.prefab");
            
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName == "Stigma")
                continue;

            GameObject go = GameManager.Resource.Load<GameObject>("Prefabs/Stigma/" + fileName);
            Stigma stigma = go.GetComponent<Stigma>();

            if (stigma.IsSpecial)
                unique.Add(stigma);
            else if (stigma.IsLock)
                locked.Add(stigma);
            else
            {
                common.Add(stigma);
                AddToWeightList(stigma, GetWeight(stigma));
            }
                

            entire.Add(stigma);
        }

        StigmaDict.Add(StigmaManageType.Common, common);
        StigmaDict.Add(StigmaManageType.Unique, unique);
        StigmaDict.Add(StigmaManageType.Locked, locked);
        StigmaDict.Add(StigmaManageType.Entire, entire);
    }

    public Stigma GetRandomStigma()
    {
        int randNum = Mathf.FloorToInt(_weightStigmaList.Count * Random.Range(0f, 1f));
        return _weightStigmaList[randNum];
    }

    private int GetWeight(Stigma stigma)
    {
        int weight = 0;
        weight = 4 - stigma.Tier;
        return weight;
    }

    private void AddToWeightList(Stigma stigma, int weight)
    {
        for(int i=0; i<weight; i++)
        {
            _weightStigmaList.Add(stigma);
        }
    }
}