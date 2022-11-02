using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitData unitData;
    [SerializeField] private bool _isTargeting;

    private void Update()
    {
        if (_isTargeting)
        {
            transform.position = InputManager.MousePos;
        }
    }

    public void OnClick()
    {
        _isTargeting = true;
        GetComponent<Collider2D>().enabled = false;
    }

    public void OffClick()
    {
        _isTargeting = false;
        GetComponent<Collider2D>().enabled = true;
    }
}