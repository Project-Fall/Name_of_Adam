using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StigmaController
{
    public StigmaController()
    {
        LoadStigmaList();
    }

    private List<Stigma> _tier1StigmaList = new();
    private List<Stigma> _tier2StigmaList = new();
    private List<Stigma> _tier3StigmaList = new();
    private List<Stigma> _uniqueStigmaList = new();
    private List<Stigma> _harlotStigmaList = new();

    private void LoadStigmaList()
    {
        string path = "Assets/Resources/Prefabs/Stigma";
        string[] files = Directory.GetFiles(path, "*.prefab");

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            GameObject go = GameManager.Resource.Load<GameObject>("Prefabs/Stigma/" + fileName);
            Stigma stigma = go.GetComponent<Stigma>();

            if (stigma.Tier == StigmaTier.Tier1)
            {
                _tier1StigmaList.Add(stigma);
            }
            else if (stigma.Tier == StigmaTier.Tier2)
            {
                _tier2StigmaList.Add(stigma);
            }
            else if (stigma.Tier == StigmaTier.Tier3)
            {
                _tier3StigmaList.Add(stigma);
            }
            else if (stigma.Tier == StigmaTier.Unique)
            {
                _uniqueStigmaList.Add(stigma);
            }
            else if (stigma.Tier == StigmaTier.Harlot)
            {
                _harlotStigmaList.Add(stigma);
            }
        }
    }

    public Stigma GetRandomStigma(List<int> probability)
    {
        int randNum = Random.Range(0, 100);

        if (randNum >= probability[0])
        {
            return _tier3StigmaList[Random.Range(0, _tier3StigmaList.Count)];
        }
        else if (randNum >= probability[1])
        {
            return _tier2StigmaList[Random.Range(0, _tier2StigmaList.Count)];
        }
        else
        {
            return _tier1StigmaList[Random.Range(0, _tier1StigmaList.Count)];
        }
     }
}