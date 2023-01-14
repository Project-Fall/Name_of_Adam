using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnChange : EventTrigger
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.BattleMNG.EngageMNG.TurnStart();
    }
}