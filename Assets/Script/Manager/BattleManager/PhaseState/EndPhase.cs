using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase : Phase
{
    public override void OnStateEnter()
    {
        BattleManager.Instance.FieldActiveEventCheck(ActiveTiming.TURN_END);
        BattleManager.Instance.BattleOverCheck();
    }
    public override void OnStateUpdate()
    {
        BattleManager.Phase.ChangePhase(BattleManager.Phase.Prepare);
    }

    public override void OnClickEvent(Vector2 coord)
    {
        
    }
    public override void OnStateExit()
    {
        //BattleManager.Instance.BenedictionCheck();
    }
}
