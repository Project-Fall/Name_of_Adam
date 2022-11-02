using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class InputManager : Manager<InputManager>
{
    public Action KeyAction = null;
    private static Vector2 s_mousePosition;
    public static Vector2 MousePosition { get { return s_mousePosition; } }
    private Unit target;

    public void OnUpdate()
    {
        // 실시간 마우스 위치
        s_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(MousePosition, Vector2.zero);
        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            if (hit.collider.CompareTag("Unit"))
            {
                target = hit.collider.gameObject.GetComponent<Unit>();
                target.OnTarget();
            }
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            target.OffTarget();
        }
    }
}