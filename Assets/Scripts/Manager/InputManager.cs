using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Manager<InputManager>
{
    public Action KeyAction = null;

    public void OnUpdate()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero);
        if (Input.GetMouseButton(0) && hit.collider != null)
        {
            if(hit.collider.name == "Unit")
            {
                hit.collider.gameObject.transform.position = mouse;
            }
        }
    }
}