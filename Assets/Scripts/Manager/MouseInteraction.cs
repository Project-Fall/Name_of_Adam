using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스로 상호작용 가능한 클래스들의 부모 클래스입니다.
/// </summary>
public class MouseInteraction : MonoBehaviour, IMouseInteraction
{
    public virtual void OffClick()
    {
        return;
    }

    public virtual void OffHover()
    {
        return;
    }

    public virtual void OnClick()
    {
        return;
    }

    public virtual void OnHover()
    {
        return;
    }
}
