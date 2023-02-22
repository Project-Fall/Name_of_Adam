using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extention
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
}