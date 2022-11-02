using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class InputManager : Manager<InputManager>
{
    private static Vector2 s_mousePosition;
    public static Vector2 MousePosition { get { return s_mousePosition; } }
    private MouseInteraction target;

    public void OnUpdate()
    {
        // 실시간 마우스 위치
        s_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(MousePosition, Vector2.zero);

        // 좌클릭 시
        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            target = hit.collider.gameObject.GetComponent<MouseInteraction>();
            target.OnClick();
            return;
        }

        // 마우스 뗐을 때
        if (Input.GetMouseButtonUp(0))
        {
            target.OffClick();
            target = null;
        }
    }
}