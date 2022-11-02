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

    public override void OnClick()
    {
        _isTargeting = true;
    }

    public override void OffClick()
    {
        _isTargeting = false;
    }
}