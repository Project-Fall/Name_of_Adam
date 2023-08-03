using System.Collections;
using UnityEngine;

public class 고문관 : Stigma
{
    public override void Use(BattleUnit caster, BattleUnit receiver)
    {
        Vector2 moveVec = (receiver.Location - caster.Location).normalized;

        if (!BattleManager.Field.TileDict[caster.Location + moveVec].UnitExist)
        {
            base.Use(caster, receiver);
            Debug.Log(caster.Location + moveVec);
            BattleManager.Field.MoveUnit(receiver.Location, caster.Location + moveVec);
        }
    }
}