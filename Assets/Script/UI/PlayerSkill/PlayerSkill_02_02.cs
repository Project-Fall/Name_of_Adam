using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill_02_02 : PlayerSkill
{
    public override bool Use(Vector2 coord)
    {
        BattleUnit targetUnit = BattleManager.Field.GetUnit(coord);
        //GameManager.Sound.Play("UI/PlayerSkillSFX/Fall");
        GameManager.VisualEffect.StartVisualEffect("Arts/EffectAnimation/PlayerSkill/DarkThunder", BattleManager.Field.GetTilePosition(coord));

        targetUnit.SetBuff(new Buff_Vice());
        targetUnit.ChangeFall(1);
        return false;
    }

    public override void CancelSelect()
    {
        BattleManager.PlayerSkillController.PlayerSkillReady(FieldColorType.none);
    }

    public override void OnSelect()
    {
        BattleManager.PlayerSkillController.PlayerSkillReady(FieldColorType.PlayerSkill, PlayerSkillTargetType.Friendly);
    }
}