using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    void Update()
    {
        _input.OnUpdate();
    }
}
