using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MouseInteraction
{
    [SerializeField] public UnitData unitData;
    [SerializeField] private bool _isTargeting;

    private void Update()
    {
        if (_isTargeting)
        {
            transform.position = InputManager.MousePosition;
        }
    }

    public void OnTarget()
    {
        _isTargeting = true;
    }

    public void OffTarget()
    {
        _isTargeting = false;
    }
}