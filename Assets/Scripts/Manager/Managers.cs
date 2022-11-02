using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    MapManager _map = new MapManager();
    public static MapManager Map { get { return Instance._map; } }

    protected override void Awake()
    {
        base.Awake();
        _map.Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }
}
