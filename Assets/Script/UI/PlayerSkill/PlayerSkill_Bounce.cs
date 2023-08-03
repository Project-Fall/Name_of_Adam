using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill_Bounce : PlayerSkill
{
    public override void Use(Vector2 coord)
    {
        //GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        //이팩트를 여기에 추가

        BattleUnit unit = BattleManager.Field.GetUnit(coord);

        BattleManager.Data.BattleUnitRemove(unit);
        BattleManager.Data.BattleOrderRemove(unit);
        BattleManager.Data.AddDeckUnit(unit.DeckUnit);
        BattleManager.BattleUI.FillHand();
        BattleManager.Field.FieldCloseInfo(BattleManager.Field.TileDict[coord]);
        Destroy(unit.gameObject);
    }

    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.FriendlyTargetPlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.FriendlyTargetPlayerSkillReady(FieldColorType.PlayerSkill);
    }
}