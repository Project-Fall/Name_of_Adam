using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation Data", menuName = "Scriptable Object/Conversation Data")]
public class ConversationData : ScriptableObject
{
    [SerializeField] private Dictionary<string, Script> scriptDict;
}

[Serializable]
public class Script
{
    public string name;
    public string script;
}

[Serializable]
public class Content
{
    public string title;
    public List<Script> contents = new List<Script>();
}

[Serializable]
public class StageList
{
    public int Level;
    public List<StageSpawnData> StageData;
}

[Serializable]
public class StageSpawnData
{
    public int ID;
    public List<StageUnitData> Units;
}

[Serializable]
public class StageUnitData
{
    public string Name;
    public Vector2 Location;
}


[Serializable]
public class ScriptLoader : ILoader<string, List<Script>>
{
    public List<Content> scripts = new List<Content>();

    public Dictionary<string, List<Script>> MakeDict()
    {
        Dictionary<string, List<Script>> dic = new Dictionary<string, List<Script>>();
        foreach (Content content in scripts)
        {
            dic.Add(content.title, content.contents);
        }
        return dic;
    }

}
[Serializable]
public class StageLoader : ILoader<int, List<StageSpawnData>>
{
    public List<StageList> StageList = new List<StageList>();

    public Dictionary<int, List<StageSpawnData>> MakeDict()
    {
        Dictionary<int, List<StageSpawnData>> dic = new Dictionary<int, List<StageSpawnData>>();
        foreach (StageList stage in StageList)
        {
            dic.Add(stage.Level, stage.StageData);
        }
        return dic;
    }
}