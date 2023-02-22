using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] List<Image> Buttons;

    public void MouseEnter()
    {
        foreach(Image img in Buttons)
        {
            img.color = Color.black;
        }
    }
    public void MouseExit()
    {
        foreach (Image img in Buttons)
        {
            img.color = Color.red;
        }
    }
}
