using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInteraction
{
    public abstract void OnClick();
    public abstract void OffClick();
    public abstract void OnHover();
    public abstract void OffHover();
}
