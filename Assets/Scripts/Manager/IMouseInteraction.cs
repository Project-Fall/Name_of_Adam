using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInteraction
{
    public void OnClick();
    public void OffClick();
    public void OnHover();
    public void OffHover();
}
