using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnChange : MonoBehaviour
{
    void OnMouseDown()
    {
        GameManager.Instance.BattleMNG.TurnStart();
    }
}