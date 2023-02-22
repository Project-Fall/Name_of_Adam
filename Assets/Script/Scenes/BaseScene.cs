using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    //public Scene SceneType { get; protected set; } = Scene.Main;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            GameManager.Resource.Instantiate("UI/EventSystem").name = "EventSystem";
    }

    /// <summary>
    /// 씬이 꺼지기 전에 해야하는 작업들
    /// </summary>
    public abstract void Clear();
}