using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaitingUnit : MonoBehaviour
{
    private Image _Image;

    void Awake()
    {
        _Image = GetComponent<Image>();
    }

    public void SetUnit(Sprite s)
    {
        
        _Image.enabled = true;
        _Image.sprite = s;
        
    }

    public void RemoveUnit()
    {
        _Image.enabled = false;
    }
}
