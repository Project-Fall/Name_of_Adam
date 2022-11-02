using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Manager<InputManager>
{
    private static Vector2 s_mousePos;
    public static Vector2 MousePos { get { return s_mousePos; } }

    private static Unit _clickTarget;
    public static Unit ClickTarget { get { return _clickTarget; } }

    private static Tile _hoverTarget;
    public static Tile HoverTarget { get { return _hoverTarget; } set { _hoverTarget = value; } }

    public static bool IsClick = false;

    public void OnUpdate()
    {
        // 실시간 마우스 위치
        s_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector2.zero);

        // 좌클릭 시
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider.CompareTag("Unit"))
            {
                _clickTarget = hit.collider.gameObject.GetComponent<Unit>();
                _clickTarget.OnClick();
                IsClick = true;
                return;
            }
        }

        // 마우스 뗐을 때
        if (Input.GetMouseButtonUp(0) && _clickTarget != null)
        {
            _clickTarget.OffClick();

            if (HoverTarget != null)
                HoverTarget.DeployUnit(ClickTarget);

            _clickTarget = null;
            IsClick = false;
        }
    }
}